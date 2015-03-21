﻿using System;
using System.Collections.ObjectModel;
using RayTracer.Helpers;
using RayTracer.Model.Shapes;

namespace RayTracer.Model
{
    public class Cursor3D : ShapeBase
    {
        #region Private Members
        public double _xPosition;
        public double _yPosition;
        public double _zPosition;
        private static Cursor3D _instance;
        private const double CursorSize = 0.1;
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
        #endregion Public Properties
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Cursor3D"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public Cursor3D(double x, double y, double z)
            : base(x, y, z)
        {
            SetVertices();
            SetEdges();
            TransformVertices();
        }
        #endregion Constructors
        #region Private Methods
        /// <summary>
        /// Sets the vertices.
        /// </summary>
        private void SetVertices()
        {
            Vertices.Add(new Vector4(-CursorSize, 0, 0, 1));
            Vertices.Add(new Vector4(CursorSize, 0, 0, 1));
            Vertices.Add(new Vector4(0, CursorSize, 0, 1));
            Vertices.Add(new Vector4(0, -CursorSize, 0, 1));
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
        #region Public Methods
        #endregion Public Methods
        #region Protected Methods

        protected override void TransformVertices()
        {
            base.TransformVertices();
            if (TransformedVertices.Count == 0) return;
            Vector4 position = ModelTransform * (Vertices[0] + ((Vertices[1] - Vertices[0]) / 2));
            XPosition = position.X;
            YPosition = position.Y;
            ZPosition = position.Z;
        }

        #endregion Protected Methods
    }
}
