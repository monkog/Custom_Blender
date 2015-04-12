using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Media.Media3D;
using RayTracer.Helpers;
using RayTracer.ViewModel;

namespace RayTracer.Model.Shapes
{
    public sealed class PointEx : ShapeBase
    {
        #region Private Members
        private bool _isCurvePointSelected;
        #endregion Private Members
        #region Public Properties
        public bool IsCurvePointSelected
        {
            get { return _isCurvePointSelected; }
            set
            {
                if (_isCurvePointSelected == value) return;
                _isCurvePointSelected = value;
                OnPropertyChanged("IsCurvePointSelected");
            }
        }
        public Vector4 Vector4 { get { return new Vector4(X, Y, Z, 1); } }
        /// <summary>
        /// Gets or sets the vertices representing the mesh.
        /// Vertices are transformed using the current matrix.
        /// </summary>
        public Vector4 TransformedPosition { get; set; }
        /// <summary>
        /// Gets or sets the point's position on the screen.
        /// </summary>
        public Vector4 PointOnScreen { get; set; }
        /// <summary>
        /// Gets or sets the vertex transform.
        /// </summary>
        public Matrix3D MeshTransform { get; set; }
        #endregion Public Properties
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PointEx"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public PointEx(double x, double y, double z)
            : base(x, y, z, "Point(" + x + ", " + y + ", " + z + ")")
        {
            Thickness = 3;
            PropertyChanged += PointEx_PropertyChanged;
            IsCurvePointSelected = false;
            
            CalculateShape();
        }
        #endregion Constructors
        #region Private Methods
        /// <summary>
        /// Sets the cursor to the point's position if it's the only one point selected
        /// </summary>
        private void PointEx_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsSelected":
                    if (PointManager.Instance.SelectedItems.Count() != 1) return;
                    var transformedPoint = ModelTransform * Vector4;

                    var cursorPosition = new Vector3D(Cursor3D.Instance.XPosition, Cursor3D.Instance.YPosition,
                        Cursor3D.Instance.ZPosition);
                    var delta = new Vector3D(transformedPoint.X, transformedPoint.Y, transformedPoint.Z) - cursorPosition;
                    Cursor3D.Instance.ModelTransform = Transformations.TranslationMatrix(delta) *
                                                       Cursor3D.Instance.ModelTransform;
                    break;
            }
        }
        #endregion Private Methods
        #region Protected Methods
        /// <summary>
        /// Transforms the point position using the current transform matrix
        /// </summary>
        protected override void CalculateShape()
        {
            //TransformedPosition = Transformations.TransformPoint(Vector4, ModelTransform).Normalized;
            TransformedPosition = MeshTransform * ModelTransform * Vector4;
            PointOnScreen = Transform * TransformedPosition;
        }
        #endregion Protected Methods
        #region Public Methods
        public void Draw()
        {
            Bitmap bmp = SceneManager.Instance.SceneImage;

            using (Graphics g = Graphics.FromImage(bmp))
            {
                if (SceneManager.Instance.IsStereoscopic)
                {
                    Color color;
                    Transform =  Transformations.StereographicLeftViewMatrix(20, 400);
                    if (!(PointOnScreen.X < 0 || PointOnScreen.X > bmp.Width || PointOnScreen.Y < 0 || PointOnScreen.Y > bmp.Height))
                    {
                        color = bmp.GetPixel((int)PointOnScreen.X, (int)PointOnScreen.Y);
                        g.FillRectangle(new SolidBrush(color.CombinedColor(Color.Red)), (int)PointOnScreen.X, (int)PointOnScreen.Y, Thickness, Thickness);
                    }

                    Transform = Transformations.StereographicRightViewMatrix(20, 400);
                    if (PointOnScreen.X < 0 || PointOnScreen.X > bmp.Width || PointOnScreen.Y < 0 || PointOnScreen.Y > bmp.Height) return;
                    color = bmp.GetPixel((int)PointOnScreen.X, (int)PointOnScreen.Y);
                    g.FillRectangle(new SolidBrush(color.CombinedColor(Color.Blue)), (int)PointOnScreen.X, (int)PointOnScreen.Y, Thickness, Thickness);
                }
                else
                {
                    Transform = Transformations.ViewMatrix(400);
                    if (PointOnScreen.X < 0 || PointOnScreen.X > bmp.Width || PointOnScreen.Y < 0 || PointOnScreen.Y > bmp.Height) return;
                    Color color = bmp.GetPixel((int)PointOnScreen.X, (int)PointOnScreen.Y);
                    g.FillRectangle(new SolidBrush(color.CombinedColor(Color.DarkCyan)), (int)PointOnScreen.X, (int)PointOnScreen.Y, Thickness, Thickness);
                }
            }
            SceneManager.Instance.SceneImage = bmp;
        }
        #endregion Public Methods
    }
}

