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
        private const double Size = 0.05;
        private bool _isSelected;
        #endregion Private Members
        #region Public Properties
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }
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
            SetVertices();
            SetEdges();
            IsSelected = false;
            PropertyChanged += PointEx_PropertyChanged;
        }
        #endregion Constructors
        #region Private Methods
        /// <summary>
        /// Sets the vertices.
        /// </summary>
        private void SetVertices()
        {
            Vertices.Add(new Vector4(X, Y, Z, 1));
            Vertices.Add(new Vector4(X + Size, Y, Z, 1));
            Vertices.Add(new Vector4(X + Size, Y + Size, Z, 1));
            Vertices.Add(new Vector4(X, Y + Size, Z, 1));
            Vertices.Add(new Vector4(X + Size, Y, Z + Size, 1));
            Vertices.Add(new Vector4(X + Size, Y + Size, Z + Size, 1));
            Vertices.Add(new Vector4(X, Y + Size, Z + Size, 1));
            Vertices.Add(new Vector4(X, Y, Z + Size, 1));
        }
        /// <summary>
        /// Sets the edges.
        /// </summary>
        private void SetEdges()
        {
            EdgesIndices = new ObservableCollection<Tuple<int, int>>();
            //up
            EdgesIndices.Add(new Tuple<int, int>(0, 1));
            EdgesIndices.Add(new Tuple<int, int>(1, 2));
            EdgesIndices.Add(new Tuple<int, int>(2, 3));
            EdgesIndices.Add(new Tuple<int, int>(3, 0));
            ////back
            EdgesIndices.Add(new Tuple<int, int>(0, 7));
            EdgesIndices.Add(new Tuple<int, int>(7, 6));
            EdgesIndices.Add(new Tuple<int, int>(6, 3));
            EdgesIndices.Add(new Tuple<int, int>(3, 0));
            ////left
            EdgesIndices.Add(new Tuple<int, int>(0, 7));
            EdgesIndices.Add(new Tuple<int, int>(7, 4));
            EdgesIndices.Add(new Tuple<int, int>(4, 1));
            EdgesIndices.Add(new Tuple<int, int>(1, 0));
            //front
            EdgesIndices.Add(new Tuple<int, int>(1, 2));
            EdgesIndices.Add(new Tuple<int, int>(2, 5));
            EdgesIndices.Add(new Tuple<int, int>(5, 4));
            EdgesIndices.Add(new Tuple<int, int>(4, 1));
            //right
            EdgesIndices.Add(new Tuple<int, int>(2, 3));
            EdgesIndices.Add(new Tuple<int, int>(3, 6));
            EdgesIndices.Add(new Tuple<int, int>(6, 5));
            EdgesIndices.Add(new Tuple<int, int>(5, 2));
            //bottom
            EdgesIndices.Add(new Tuple<int, int>(5, 4));
            EdgesIndices.Add(new Tuple<int, int>(4, 7));
            EdgesIndices.Add(new Tuple<int, int>(7, 6));
            EdgesIndices.Add(new Tuple<int, int>(6, 5));
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
                    var transformedPoint = ModelTransform * new Vector4(X, Y, Z, 1);

                    var cursorPosition = new Vector3D(Cursor3D.Instance.XPosition, Cursor3D.Instance.YPosition,
                        Cursor3D.Instance.ZPosition);
                    var delta = new Vector3D(transformedPoint.X, transformedPoint.Y, transformedPoint.Z) - cursorPosition;
                    Cursor3D.Instance.ModelTransform = Transformations.TranslationMatrix(delta) *
                                                       Cursor3D.Instance.ModelTransform;
                    break;
            }
        }
        #endregion Private Methods
        #region Public Methods
        #endregion Public Methods
    }
}

