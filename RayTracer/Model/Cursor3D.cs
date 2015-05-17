using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;
using RayTracer.Helpers;
using RayTracer.Model.Shapes;

namespace RayTracer.Model
{
    public sealed class Cursor3D : ModelBase
    {
        #region Private Members
        public double _xPosition;
        public double _yPosition;
        public double _zPosition;
        private double _xScreenPosition;
        private double _yScreenPosition;
        private static Cursor3D _instance;
        private const double CursorSize = 0.2;
        #endregion Private Members
        #region Public Properties
        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static Cursor3D Instance { get { return _instance ?? (_instance = new Cursor3D(0, 0, 0)); } }
        /// <summary>
        /// Gets or sets the x position of the cursor in 3D space.
        /// </summary>
        /// <value>
        /// The x position of the cursor in 3D space.
        /// </value>
        public double XPosition
        {
            get { return _xPosition; }
            set
            {
                if (_xPosition == value) return;
                _xPosition = value;
                OnPropertyChanged("XPosition");
            }
        }
        /// <summary>
        /// Gets or sets the y position of the cursor in 3D space.
        /// </summary>
        /// <value>
        /// The y position of the cursor in 3D space.
        /// </value>
        public double YPosition
        {
            get { return _yPosition; }
            set
            {
                if (_yPosition == value) return;
                _yPosition = value;
                OnPropertyChanged("YPosition");
            }
        }
        /// <summary>
        /// Gets or sets the z position of the cursor in 3D space.
        /// </summary>
        /// <value>
        /// The z position of the cursor in 3D space.
        /// </value>
        public double ZPosition
        {
            get { return _zPosition; }
            set
            {
                if (_zPosition == value) return;
                _zPosition = value;
                OnPropertyChanged("ZPosition");
            }
        }
        /// <summary>
        /// Gets or sets the x Screen position of the cursor in 3D space.
        /// </summary>
        /// <value>
        /// The x Screen position of the cursor in 3D space.
        /// </value>
        public double XScreenPosition
        {
            get { return _xScreenPosition; }
            set
            {
                if (_xScreenPosition == value) return;
                _xScreenPosition = value;
                OnPropertyChanged("XScreenPosition");
            }
        }
        /// <summary>
        /// Gets or sets the y Screen position of the cursor in 3D space.
        /// </summary>
        /// <value>
        /// The y Screen position of the cursor in 3D space.
        /// </value>
        public double YScreenPosition
        {
            get { return _yScreenPosition; }
            set
            {
                if (_yScreenPosition == value) return;
                _yScreenPosition = value;
                OnPropertyChanged("YScreenPosition");
            }
        }
        public override IEnumerable<ShapeBase> SelectedItems
        {
            get { return new List<ShapeBase>(); }
        }
        #endregion Public Properties
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Cursor3D"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public Cursor3D(double x, double y, double z)
            : base(x, y, z, string.Empty)
        {
            SetVertices();
            SetEdges();
            CalculateShape();
            DisplayVertices = false;
        }
        #endregion Constructors
        #region Private Methods
        /// <summary>
        /// Sets the vertices.
        /// </summary>
        private void SetVertices()
        {
            Vertices.Add(new PointEx(-CursorSize, 0, 0));
            Vertices.Add(new PointEx(CursorSize, 0, 0));
            Vertices.Add(new PointEx(0, CursorSize, 0));
            Vertices.Add(new PointEx(0, -CursorSize, 0));
        }
        /// <summary>
        /// Sets the edges.
        /// </summary>
        private void SetEdges()
        {
            EdgesIndices = new ObservableCollection<Tuple<int, int>>();

            EdgesIndices.Add(new Tuple<int, int>(0, 1));
            EdgesIndices.Add(new Tuple<int, int>(2, 3));
        }
        #endregion Private Methods
        #region Protected Methods        
        /// <summary>
        /// Transforms the vertices.
        /// </summary>
        protected override void TransformVertices(Matrix3D transform)
        {
            base.TransformVertices(transform);
            Vector4 position = (Vertices[0].TransformedPosition + ((Vertices[1].TransformedPosition - Vertices[0].TransformedPosition) / 2));
            XPosition = position.X;
            YPosition = position.Y;
            ZPosition = position.Z;
            Vector4 transformPosition = (Vertices[0].PointOnScreen + ((Vertices[1].PointOnScreen - Vertices[0].PointOnScreen) / 2));
            XScreenPosition = transformPosition.X;
            YScreenPosition = transformPosition.Y;
        }
        #endregion Protected Methods
    }
}
