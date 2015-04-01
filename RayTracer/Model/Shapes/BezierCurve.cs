using System;
using System.Collections.Generic;
using RayTracer.Helpers;

namespace RayTracer.Model.Shapes
{
    public sealed class BezierCurve : ShapeBase
    {
        #region Private Methods
        #endregion Private Methods
        #region Public Properties
        public Continuity Continuity { get; private set; }
        #endregion Public Properties
        #region Constructors

        public BezierCurve(double x, double y, double z, string name, IEnumerable<PointEx> points, Continuity continuity)
            : base(x, y, z, name)
        {
            Continuity = continuity;

            SetVertices(points);
            SetEdges();
            TransformVertices();
        }
        #endregion Constructors
        #region Private Methods
        private void SetVertices(IEnumerable<PointEx> points)
        {
            if (Continuity == Continuity.C0)
                foreach (var point in points)
                    Vertices.Add(new Vector4(point.X, point.Y, point.Z, 1));
        }
        private void SetEdges()
        {
            EdgesIndices.Add(new Tuple<int, int>(0, 1));
            EdgesIndices.Add(new Tuple<int, int>(1, 2));
            EdgesIndices.Add(new Tuple<int, int>(2, 3));
        }
        #endregion Private Methods
        #region Public Methods
        #endregion Public Methods
    }
    /// <summary>
    /// The continuity of the Bezier curve
    /// </summary>
    public enum Continuity
    {
        C0, C1, C2
    }
}

