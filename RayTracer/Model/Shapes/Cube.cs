using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using RayTracer.Helpers;

namespace RayTracer.Model.Shapes
{
    public class Cube : ShapeBase
    {
        #region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="Cube"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <param name="size">The size of the cube.</param>
        public Cube(double x, double y, double z, int size)
            : base(x, y, z)
        {
            Width = Height = Depth = size;
            SetVertices();
            SetEdges();
            TransformVertices();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Cube"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="size">The size of the cube.</param>
        public Cube(Point3D position, int size)
            : this(position.X, position.Y, position.Z, size)
        { }
        #endregion .ctor
        #region Public Properties
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }
        #endregion Public Properties
        #region Private Methods
        /// <summary>
        /// Sets the vertices.
        /// </summary>
        private void SetVertices()
        {
            Vertices.Add(new Vector4(X, Y, Z, 0));
            Vertices.Add(new Vector4(X + Width, Y, Z, 0));
            Vertices.Add(new Vector4(X + Width, Y + Height, Z, 0));
            Vertices.Add(new Vector4(X, Y + Height, Z, 0));
            Vertices.Add(new Vector4(X + Width, Y, Z + Depth, 0));
            Vertices.Add(new Vector4(X + Width, Y + Height, Z + Depth, 0));
            Vertices.Add(new Vector4(X, Y + Height, Z + Depth, 0));
            Vertices.Add(new Vector4(X, Y, Z + Depth, 0));
        }
        /// <summary>
        /// Sets the edges.
        /// </summary>
        private void SetEdges()
        {
            EdgesIndices = new ObservableCollection<Tuple<int, int>>();
            EdgesIndices.Add(new Tuple<int, int>(0, 1));
            EdgesIndices.Add(new Tuple<int, int>(1, 2));
            EdgesIndices.Add(new Tuple<int, int>(2, 3));
            EdgesIndices.Add(new Tuple<int, int>(3, 0));
        }
        #endregion Private Methods
    }
}
