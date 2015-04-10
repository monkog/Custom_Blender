using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Media.Media3D;
using RayTracer.Helpers;
using RayTracer.ViewModel;

namespace RayTracer.Model.Shapes
{
    /// <summary>
    /// Base class for shape objects
    /// </summary>
    public abstract class ShapeBase : ViewModelBase
    {
        #region Private Members
        private bool _isSelected;
        private Matrix3D _transform;
        private Matrix3D _modelTransform;
        private string _name;
        #endregion Private Members
        #region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="Cube"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <param name="name">Name of the mesh</param>
        protected ShapeBase(double x, double y, double z, string name)
        {
            X = x;
            Y = y;
            Z = z;

            Name = name;
            IsSelected = false;

            Vertices = new ObservableCollection<Vector4>();
            TransformedVertices = new ObservableCollection<Vector4>();
            Edges = new ObservableCollection<CustomLine>();
            EdgesIndices = new ObservableCollection<Tuple<int, int>>();
            _transform = Transformations.Identity;
            ModelTransform = Transformations.Identity;
        }
        #endregion .ctor
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
        /// <summary>
        /// Gets or sets the x position.
        /// </summary>
        /// <value>
        /// The x position.
        /// </value>
        public double X { get; set; }
        /// <summary>
        /// Gets or sets the y position.
        /// </summary>
        /// <value>
        /// The y position.
        /// </value>
        public double Y { get; set; }
        /// <summary>
        /// Gets or sets the z position.
        /// </summary>
        /// <value>
        /// The z position.
        /// </value>
        public double Z { get; set; }
        /// <summary>
        /// Gets or sets the vertices representing the mesh.
        /// </summary>
        /// <value>
        /// The vertices representing the mesh.
        /// </value>
        public ObservableCollection<Vector4> Vertices { get; set; }
        /// <summary>
        /// Gets or sets the vertices representing the mesh.
        /// Vertices are transformed using the current matrix.
        /// </summary>
        /// <value>
        /// The vertices representing the mesh.
        /// </value>
        public ObservableCollection<Vector4> TransformedVertices { get; set; }
        /// <summary>
        /// Gets or sets the edges representing the mesh.
        /// </summary>
        /// <value>
        /// The edges representing the mesh.
        /// </value>
        public ObservableCollection<CustomLine> Edges { get; protected set; }
        /// <summary>
        /// Gets or sets the transform multiplied by the projection transformations.
        /// </summary>
        /// <value>
        /// The transform multiplied by the projection transformations.
        /// </value>
        public Matrix3D Transform
        {
            get { return _transform; }
            set
            {
                _transform = SceneManager.Instance.TransformMatrix * SceneManager.Instance.ScaleMatrix * value * ModelTransform;
                CalculateShape();
            }
        }
        /// <summary>
        /// Gets or sets the current transform of the model.
        /// </summary>
        /// <value>
        /// The current transform of the model.
        /// </value>
        public Matrix3D ModelTransform
        {
            get { return _modelTransform; }
            set
            {
                if (_modelTransform == value)
                    return;
                _modelTransform = value;
                OnPropertyChanged("ModelTransform");
            }
        }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        #endregion Public Properties
        #region Protected Properties
        /// <summary>
        /// Gets or sets the edges indices.
        /// </summary>
        /// <value>
        /// The edges indices.
        /// </value>
        protected ObservableCollection<Tuple<int, int>> EdgesIndices { get; set; }
        /// <summary>
        /// Used for synchronising bitmap access
        /// </summary>
        protected readonly object BitmapLock = new object();
        /// <summary>
        /// The thickness of a line drawing the shape
        /// </summary>
        protected int Thickness = 1;
        #endregion Protected Properties
        #region Protected Methods
        /// <summary>
        /// Calculates the shape: vertices and edges
        /// </summary>
        protected virtual void CalculateShape()
        {
            TransformVertices();
            TransformEdges();
        }
        /// <summary>
        /// Transforms the vertices.
        /// </summary>
        protected virtual void TransformVertices()
        {
            TransformedVertices = new ObservableCollection<Vector4>();
            foreach (var vertex in Vertices)
                TransformedVertices.Add(Transformations.TransformPoint(vertex, Transform).Normalized);
        }
        /// <summary>
        /// Transforms the edges of the mesh.
        /// </summary>
        protected void TransformEdges()
        {
            Edges = new ObservableCollection<CustomLine>();
            foreach (var edge in EdgesIndices)
            {
                var begining = TransformedVertices[edge.Item1];
                var end = TransformedVertices[edge.Item2];
                Edges.Add(new CustomLine(new Point((int)begining.X, (int)begining.Y), new Point((int)end.X, (int)end.Y)));
            }
            OnPropertyChanged("Edges");
        }
        /// <summary>
        /// Draws the shape.
        /// </summary>
        /// <param name="bmp">The bitmap to draw onto.</param>
        /// <param name="graphics">The graphics of the bitmap</param>
        /// <param name="color">The color of the shape</param>
        /// <param name="thickness">thickness of the line</param>
        protected void DrawShape(Bitmap bmp, Graphics graphics, Color color, int thickness)
        {
            foreach (var edge in Edges)
                edge.Draw(bmp, graphics, color, thickness);
        }
        #endregion Protected Methods
        #region Public Methods
        /// <summary>
        /// Draws this instance.
        /// </summary>
        public virtual void Draw()
        {
            Bitmap bmp = SceneManager.Instance.SceneImage;

            using (Graphics g = Graphics.FromImage(bmp))
            {
                if (SceneManager.Instance.IsStereoscopic)
                {
                    Transform = Transformations.StereographicLeftViewMatrix(20, 400);
                    DrawShape(bmp, g, Color.Red, Thickness);
                    Transform = Transformations.StereographicRightViewMatrix(20, 400);
                    DrawShape(bmp, g, Color.Blue, Thickness);
                }
                else
                {
                    Transform = Transformations.ViewMatrix(400);
                    DrawShape(bmp, g, Color.DarkCyan, Thickness);
                }
            }
            SceneManager.Instance.SceneImage = bmp;
        }
        #endregion Public Methods
    }
}
