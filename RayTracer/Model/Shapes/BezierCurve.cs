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
            for (int i = 0; i < Vertices.Count; i++)
                EdgesIndices.Add(new Tuple<int, int>(i, Math.Min(i + 1, Vertices.Count - 1)));
        }
        private void DrawBezierCurve()
        {

        }
        #endregion Private Methods
        #region Public Methods
        public override void Draw()
        {
            DrawBezierCurve();
            base.Draw();
        }
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

