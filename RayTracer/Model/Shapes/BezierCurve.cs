using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Media.Media3D;
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
        public IEnumerable<PointEx> SelectedItems { get { return Vertices.Where(p => p.IsCurvePointSelected); } }
        #endregion Public Properties
        #region Constructors
        public BezierCurve(double x, double y, double z, string name, IEnumerable<PointEx> points)
            : base(x, y, z, name)
        {
            SetVertices(points);
            SetEdges();
            TransformVertices(Matrix3D.Identity);
            DisplayVertices = false;
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
                var point = Casteljeu(curve, t);

                if (SceneManager.Instance.IsStereoscopic)
                {
                    Color color;
                    var p = SceneManager.Instance.TransformMatrix * SceneManager.Instance.ScaleMatrix * Transformations.StereographicLeftViewMatrix(20, 400) * point;
                    if (!(p.X < 0 || p.X > bmp.Width || p.Y < 0 || p.Y > bmp.Height))
                    {
                        color = bmp.GetPixel((int)p.X, (int)p.Y);
                        g.FillRectangle(new SolidBrush(color.CombinedColor(Color.Red)), (int)p.X, (int)p.Y, Thickness, Thickness);
                    }

                    p = SceneManager.Instance.TransformMatrix * SceneManager.Instance.ScaleMatrix * Transformations.StereographicRightViewMatrix(20, 400) * point;
                    if (p.X < 0 || p.X > bmp.Width || p.Y < 0 || p.Y > bmp.Height) continue;
                    color = bmp.GetPixel((int)p.X, (int)p.Y);
                    g.FillRectangle(new SolidBrush(color.CombinedColor(Color.Blue)), (int)p.X, (int)p.Y, Thickness, Thickness);
                }
                else
                {
                    var p = SceneManager.Instance.TransformMatrix * SceneManager.Instance.ScaleMatrix * Transformations.ViewMatrix(400) * point;
                    if (p.X < 0 || p.X > bmp.Width || p.Y < 0 || p.Y > bmp.Height) continue;
                    Color color = bmp.GetPixel((int)p.X, (int)p.Y);
                    g.FillRectangle(new SolidBrush(color.CombinedColor(Color.DarkCyan)), (int)p.X, (int)p.Y, Thickness, Thickness);
                }
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
        protected void SetEdges()
        {
            EdgesIndices = new ObservableCollection<Tuple<int, int>>();
            for (int i = 0; i < Vertices.Count - 1; i++)
                EdgesIndices.Add(new Tuple<int, int>(i, i + 1));
        }
        #region Protected Methods
        /// <summary>
        /// Gets the list of points creating curves
        /// </summary>
        /// <returns>The list of points creating curves</returns>
        protected abstract List<Tuple<List<Vector4>, double>> GetBezierCurves();
        #endregion Protected Methods
        #region Public Methods
        public override void Draw()
        {
            DrawBezierCurve();
            base.Draw();
        }
        #endregion Public Methods
    }
}

