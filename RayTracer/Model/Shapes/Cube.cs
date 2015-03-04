using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;
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
        public Cube(double x, double y, double z, double size)
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
            Vertices.Add(new Vector4(X, Y, Z, 1));
            Vertices.Add(new Vector4(X + Width, Y, Z, 1));
            Vertices.Add(new Vector4(X + Width, Y + Height, Z, 1));
            Vertices.Add(new Vector4(X, Y + Height, Z, 1));
            Vertices.Add(new Vector4(X + Width, Y, Z + Depth, 1));
            Vertices.Add(new Vector4(X + Width, Y + Height, Z + Depth, 1));
            Vertices.Add(new Vector4(X, Y + Height, Z + Depth, 1));
            Vertices.Add(new Vector4(X, Y, Z + Depth, 1));
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
        #endregion Private Methods
    }
}
