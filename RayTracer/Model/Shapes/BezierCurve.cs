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
    public sealed class BezierCurve : ShapeBase
    {
        #region Private Members
        private static Matrix3D _bernsterinBasis = new Matrix3D(-1, 3, -3, 1
                                                               , 3, -6, 3, 0
                                                               , -3, 3, 0, 0
                                                               , 1, 0, 0, 0);
        private ObservableCollection<Tuple<int, int>> _bezierPolygonIndices;
        private bool _isPolygonVisible;
        #endregion Private Members
        #region Public Properties
        /// <summary>
        /// Gets the continuity of the Bezier curve.
        /// </summary>
        public Continuity Continuity { get; private set; }
        /// <summary>
        /// Gets or sets the points of the Bezier curve.
        /// </summary>
        public ObservableCollection<PointEx> Points { get; set; }
        /// <summary>
        /// Gets the selected items, that can be removed from the Bezier curve.
        /// </summary>
        public IEnumerable<PointEx> SelectedItems { get { return Points.Where(p => p.IsCurvePointSelected); } }
        /// <summary>
        /// Gets or sets a value indicating whether this curve's polygon is visible.
        /// </summary>
        public bool IsPolygonVisible
        {
            get { return _isPolygonVisible; }
            set
            {
                if (_isPolygonVisible == value) return;
                _isPolygonVisible = value;
                OnPropertyChanged("IsPolygonVisible");
            }
        }
        #endregion Public Properties
        #region Constructors
        public BezierCurve(double x, double y, double z, string name, IEnumerable<PointEx> points, Continuity continuity)
            : base(x, y, z, name)
        {
            Continuity = continuity;
            _bezierPolygonIndices = new ObservableCollection<Tuple<int, int>>();
            _isPolygonVisible = true;

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
            _bezierPolygonIndices = new ObservableCollection<Tuple<int, int>>();
            for (int i = 0; i < Points.Count; i++)
                _bezierPolygonIndices.Add(new Tuple<int, int>(i, Math.Min(i + 1, Points.Count - 1)));
        }
        protected override void TransformVertices()
        {
            TransformedVertices = new ObservableCollection<Vector4>();
            foreach (var point in Points)
                TransformedVertices.Add(Transformations.TransformPoint(point.Vertices.First(), Transform * point.ModelTransform).Normalized);
        }
        private void DrawBezierCurve()
        {
            EdgesIndices = new ObservableCollection<Tuple<int, int>>();
            if (_isPolygonVisible)
            {
                SetEdges();
                EdgesIndices = _bezierPolygonIndices;
                CalculateShape();
            }

            var curves = GetBezierCurves();
            Bitmap bmp = SceneManager.Instance.SceneImage;

            using (Graphics g = Graphics.FromImage(bmp))
                foreach (var curve in curves)
                    DrawSingleCurve(bmp, g, curve);
            SceneManager.Instance.SceneImage = bmp;
        }
        private void DrawSingleCurve(Bitmap bmp, Graphics g, List<Vector4> curve)
        {
            double divisions = GetDivisionsForBezierCurve(curve);

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

        private double GetDivisionsForBezierCurve(List<Vector4> curve)
        {
            double divisions = 0;

            for (int i = 0; i < curve.Count - 1; i++)
                divisions += (curve[i] - curve[i + 1]).Length;


            divisions *= 400;
            return 1 / divisions;
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
        /// <summary>
        /// Gets the list of points creating curves
        /// </summary>
        /// <returns>The list of points creating curves</returns>
        private List<List<Vector4>> GetBezierCurves()
        {
            var curves = new List<List<Vector4>>();
            var curve = new List<Vector4>();
            int index = 0;
            for (int i = 0; i < Points.Count(); i++)
            {
                curve.Add(Transformations.TransformPoint(Points.ElementAt(i).Vertices.First(), Points.ElementAt(i).ModelTransform).Normalized);
                index = (index + 1) % 4;

                if (index == 0 && i < Points.Count - 1)
                {
                    i--;
                    curves.Add(curve);
                    curve = new List<Vector4>();
                }
            }

            if (curve.Count > 0)
                curves.Add(curve);

            return curves;
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

