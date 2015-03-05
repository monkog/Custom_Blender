using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
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
        /// <summary>
        /// The_transform
        /// </summary>
        private Matrix3D _transform;
        /// <summary>
        /// The model transform
        /// </summary>
        private Matrix3D _modelTransform;
        #endregion Private Members
        #region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="Cube"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        protected ShapeBase(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;

            Vertices = new ObservableCollection<Vector4>();
            TransformedVertices = new ObservableCollection<Vector4>();
            Transform = Transformations.Identity;
            ModelTransform = Transformations.Identity;
        }
        #endregion .ctor
        #region Public Properties
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
        /// Gets or sets the current transform of the model.
        /// </summary>
        /// <value>
        /// The current transform of the model.
        /// </value>
        public Matrix3D Transform
        {
            get { return _transform; }
            set
            {
                if (_transform == value)
                    return;
                _transform = SceneManager.Instance.TransformMatrix * SceneManager.Instance.ScaleMatrix * value * ModelTransform;
                TransformVertices();
                OnPropertyChanged("TransformedVertices");
            }
        }
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
        #endregion Public Properties
        #region Protected Properties
        /// <summary>
        /// Gets or sets the edges indices.
        /// </summary>
        /// <value>
        /// The edges indices.
        /// </value>
        protected ObservableCollection<Tuple<int, int>> EdgesIndices { get; set; }
        #endregion Protected Properties
        #region Protected Methods
        /// <summary>
        /// Transforms the vertices.
        /// </summary>
        protected void TransformVertices()
        {
            TransformedVertices = new ObservableCollection<Vector4>();
            double min = double.MaxValue, max = double.MinValue;
            foreach (var vertex in Vertices)
            {
                var transformed = Transformations.TransformPoint(vertex, ModelTransform).Normalized;
                if (transformed.Z < - 5)
                    TransformedVertices.Add(null);
                else
                TransformedVertices.Add(Transformations.TransformPoint(vertex, Transform).Normalized);
                if (transformed.Z < min)
                    min = transformed.Z;
                if (transformed.Z > max)
                    max = transformed.Z;
            }
            TransformEdges();
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
                if (begining == null || end == null)
                    continue;

                var line = new CustomLine();
                line.Points = new List<Point>();
                line.Points.Add(new Point(begining.X, begining.Y));
                line.Points.Add(new Point(end.X, end.Y));
                line.IsLineVisible = true;
                Edges.Add(line);
            }
            OnPropertyChanged("Edges");
        }
        #endregion Protected Methods
    }
}
