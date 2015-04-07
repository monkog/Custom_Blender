using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Documents.Serialization;
using RayTracer.Helpers;

namespace RayTracer.Model.Shapes
{
    public sealed class BezierCurve : ShapeBase
    {
        #region Private Members
        #endregion Private Members
        #region Public Properties
        public Continuity Continuity { get; private set; }
        public ObservableCollection<PointEx> Points { get; set; }
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
            Points = new ObservableCollection<PointEx>(points);
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

