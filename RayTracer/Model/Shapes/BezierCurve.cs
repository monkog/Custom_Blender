using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using RayTracer.Helpers;
using RayTracer.ViewModel;

namespace RayTracer.Model.Shapes
{
    public abstract class BezierCurve : ModelBase
    {
        #region Public Properties
        /// <summary>
        /// Gets the selected items, that can be removed from the Bezier curve.
        /// </summary>
        public override IEnumerable<ShapeBase> SelectedItems { get { return Vertices.Where(p => p.IsSelected); } }
        /// <summary>
        /// Gets the continuity of the curve.
        /// </summary>
        public Continuity Continuity { get; private set; }
        #endregion Public Properties
        #region Constructors
        protected BezierCurve(double x, double y, double z, string name, IEnumerable<PointEx> points, Continuity continuity)
            : base(x, y, z, name)
        {
            SetVertices(points);
            DisplayVertices = true;
            Continuity = continuity;
        }
        #endregion Constructors
        #region Private Methods
        private void SetVertices(IEnumerable<PointEx> points)
        {
            Vertices = new ObservableCollection<PointEx>(points);
        }
        private void DrawBezierCurve()
        {
            if (DisplayEdges)
            {
                SetEdges();
                CalculateShape();
            }

            var curves = GetBezierCurves();
            Bitmap bmp = SceneManager.Instance.SceneImage;

            using (Graphics g = Graphics.FromImage(bmp))
                foreach (var curve in curves)
                    DrawSingleCurve(bmp, g, curve.Item1, curve.Item2);
            SceneManager.Instance.SceneImage = bmp;
        }
        private void DrawSingleCurve(Bitmap bmp, Graphics g, List<Vector4> curve, double divisions)
        {
            for (double t = 0; t <= 1; t += divisions)
            {
                var point = GetCurvePoint(curve, t);
                if (point == null) continue;

               SceneManager.DrawCurvePoint(bmp, g, point, Thickness);
            }
        }
        private Vector4 Casteljeu(List<Vector4> points, double t)
        {
            int counter = points.Count;
            while (points.Count < 4)
                points.Add(points.Last());
            var xValues = new Vector4(points[0].X, points[1].X, points[2].X, points[3].X);
            var yValues = new Vector4(points[0].Y, points[1].Y, points[2].Y, points[3].Y);
            var zValues = new Vector4(points[0].Z, points[1].Z, points[2].Z, points[3].Z);

            for (int i = 0; i < counter; i++)
            {
                xValues.X = xValues.X * (1 - t) + (xValues.Y * t);
                xValues.Y = xValues.Y * (1 - t) + (xValues.Z * t);
                xValues.Z = xValues.Z * (1 - t) + (xValues.A * t);

                yValues.X = yValues.X * (1 - t) + (yValues.Y * t);
                yValues.Y = yValues.Y * (1 - t) + (yValues.Z * t);
                yValues.Z = yValues.Z * (1 - t) + (yValues.A * t);

                zValues.X = zValues.X * (1 - t) + (zValues.Y * t);
                zValues.Y = zValues.Y * (1 - t) + (zValues.Z * t);
                zValues.Z = zValues.Z * (1 - t) + (zValues.A * t);
            }

            return new Vector4(xValues.X, yValues.X, zValues.X, 1);
        }
        #endregion Private Methods
        #region Protected Methods
        /// <summary>
        /// Sets the edges indices based on vertices.
        /// </summary>
        protected virtual void SetEdges()
        {
            SetEdgesInRange(from: 0, to: Vertices.Count - 1);
        }
        /// <summary>
        /// Sets the edges indices based on first and last index.
        /// </summary>
        protected void SetEdgesInRange(int from, int to)
        {
            EdgesIndices = new ObservableCollection<Tuple<int, int>>();
            for (int i = from; i < to; i++)
                EdgesIndices.Add(new Tuple<int, int>(i, i + 1));
        }
        /// <summary>
        /// Gets the list of points creating curves
        /// </summary>
        /// <returns>The list of points creating curves</returns>
        protected abstract List<Tuple<List<Vector4>, double>> GetBezierCurves();
        /// <summary>
        /// Gets the curve point to draw.
        /// </summary>
        /// <param name="curve">The curve.</param>
        /// <param name="t">The division point.</param>
        /// <returns>Coordinates to draw</returns>
        protected virtual Vector4 GetCurvePoint(List<Vector4> curve, double t)
        {
            return Casteljeu(curve, t);
        }
        #endregion Protected Methods
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
        C0, C2
    }
}

