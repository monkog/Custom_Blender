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
    public abstract class ModelBase : ShapeBase
    {
        #region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="Cube"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <param name="name">Name of the mesh</param>
        protected ModelBase(double x, double y, double z, string name)
            : base(x, y, z, name)
        {
            Vertices = new ObservableCollection<PointEx>();
            Edges = new ObservableCollection<CustomLine>();
            EdgesIndices = new ObservableCollection<Tuple<int, int>>();
            DisplayEdges = true;
            DisplayVertices = true;
        }
        #endregion .ctor
        #region Public Properties
        /// <summary>
        /// Gets or sets the vertices representing the mesh.
        /// </summary>
        /// <value>
        /// The vertices representing the mesh.
        /// </value>
        public ObservableCollection<PointEx> Vertices { get; set; }
        /// <summary>
        /// Gets or sets the edges representing the mesh.
        /// </summary>
        public ObservableCollection<CustomLine> Edges { get; protected set; }
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
        /// Gets or sets a value indicating whether the edges should be displayed.
        /// </summary>
        protected bool DisplayEdges { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the vertices should be displayed.
        /// </summary>
        protected bool DisplayVertices { get; set; }
        #endregion Protected Properties
        #region Protected Methods
        protected override void CalculateShape()
        {
            TransformVertices(Matrix3D.Identity);
            TransformEdges();
        }
        /// <summary>
        /// Transforms the vertices.
        /// </summary>
        protected virtual void TransformVertices(Matrix3D transform)
        {
            foreach (var vertex in Vertices)
            {
                vertex.Transform = transform;
                vertex.MeshTransform = ModelTransform;
            }
        }
        /// <summary>
        /// Transforms the edges of the mesh.
        /// </summary>
        protected void TransformEdges()
        {
            Edges = new ObservableCollection<CustomLine>();
            foreach (var edge in EdgesIndices)
            {
                var begining = Vertices[edge.Item1].PointOnScreen;
                var end = Vertices[edge.Item2].PointOnScreen;
                Edges.Add(new CustomLine(new Point((int)begining.X, (int)begining.Y), new Point((int)end.X, (int)end.Y)));
            }
            OnPropertyChanged("Edges");
        }
        /// <summary>
        /// Draws the edges.
        /// </summary>
        /// <param name="bmp">The bitmap to draw onto.</param>
        /// <param name="graphics">The graphics of the bitmap</param>
        /// <param name="color">The color of the shape</param>
        /// <param name="thickness">thickness of the line</param>
        protected void DrawEdges(Bitmap bmp, Graphics graphics, Color color, int thickness)
        {
            foreach (var edge in Edges)
                edge.Draw(bmp, graphics, color, thickness);
        }
        /// <summary>
        /// Draws the vertices.
        /// </summary>
        /// <param name="bmp">The BMP.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        protected void DrawVertices(Bitmap bmp, Graphics graphics, Color color, int thickness)
        {
            foreach (var vertex in Vertices)
                vertex.Draw();
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
                    if (DisplayEdges)
                        DrawEdges(bmp, g, Color.Red, Thickness);
                    if (DisplayVertices)
                        DrawVertices(bmp, g, Color.Red, Thickness);
                    Transform = Transformations.StereographicRightViewMatrix(20, 400);
                    if (DisplayEdges)
                        DrawEdges(bmp, g, Color.Blue, Thickness);
                    if (DisplayVertices)
                        DrawVertices(bmp, g, Color.Red, Thickness);
                }
                else
                {
                    Transform = Transformations.ViewMatrix(400);
                    if (DisplayEdges)
                        DrawEdges(bmp, g, Color.DarkCyan, Thickness);
                    if (DisplayVertices)
                        DrawVertices(bmp, g, Color.Red, Thickness);
                }
            }
            SceneManager.Instance.SceneImage = bmp;
        }
        #endregion Public Methods
    }
}
