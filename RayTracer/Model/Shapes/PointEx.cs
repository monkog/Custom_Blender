using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Media3D;
using RayTracer.Helpers;
using RayTracer.ViewModel;

namespace RayTracer.Model.Shapes
{
    public class PointEx : ShapeBase
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
            SetVertices(x, y, z);
            SetEdges();
            PropertyChanged += PointEx_PropertyChanged;
            IsCurvePointSelected = false;
        }
        #endregion Constructors
        #region Private Methods
        /// <summary>
        /// Sets the vertices.
        /// </summary>
        private void SetVertices(double x, double y, double z)
        {
            Vertices.Add(new Vector4(x, y, z, 1));
        }
        /// <summary>
        /// Sets the edges.
        /// </summary>
        private void SetEdges()
        {
            EdgesIndices = new ObservableCollection<Tuple<int, int>> { new Tuple<int, int>(0, 0) };
        }
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
    }
}

