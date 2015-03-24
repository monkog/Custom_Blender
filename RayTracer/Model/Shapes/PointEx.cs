using System;
using System.Collections.ObjectModel;
using RayTracer.Helpers;

namespace RayTracer.Model.Shapes
{
    public class PointEx : ShapeBase
    {
        #region Private Members
        private string _name;
        private const double Size = 0.05;
        #endregion Private Members
        #region Public Properties

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
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PointEx"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <param name="name">Name of the point</param>
        public PointEx(double x, double y, double z, string name)
            : base(x, y, z)
        {
            Name = name;
            SetVertices();
            SetEdges();
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
        #endregion Private Methods
        #region Public Methods
        #endregion Public Methods
    }
}

