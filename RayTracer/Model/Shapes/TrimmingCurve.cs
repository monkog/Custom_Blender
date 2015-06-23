using System;
using System.Collections.Generic;
using System.Linq;
using RayTracer.Helpers;
using RayTracer.ViewModel;
using Accord.Math;
using System.Drawing;

using Point = System.Windows.Point;

namespace RayTracer.Model.Shapes
{
    public class TrimmingCurve : ModelBase
    {
        #region Private Members
        private List<Point> _startPoints;
        private Bitmap _firstSurfaceImage;
        private Bitmap _secondSurfaceImage;
        private const int MaxTries = 10;
        const double NextPointDifference = 0.01;
        private List<Vector4> _curvePoints;
        #endregion Private Members
        #region Public Properties
        public override string Type
        {
            get { return "TrimmedCurve"; }
        }
        public override IEnumerable<ShapeBase> SelectedItems
        {
            get { return null; }
        }
        /// <summary>
        /// Two patches for calculating the trimming curve
        /// </summary>
        public BezierPatch[] BezierPatches { get; private set; }
        /// <summary>
        /// Gets or sets the start points for the Newton's method.
        /// </summary>
        public List<Point> StartPoints
        {
            get { return _startPoints; }
            set
            {
                if (_startPoints == value) return;
                _startPoints = value;
                OnPropertyChanged("StartPoints");
            }
        }
        /// <summary>
        /// Gets the curve points.
        /// </summary>
        public List<Vector4> CurvePoints { get { return _curvePoints; } }
        #endregion Public Properties
        #region Constructors
        public TrimmingCurve(double x, double y, double z, string name, List<BezierPatch> patches)
            : base(x, y, z, name)
        {
            _firstSurfaceImage = SceneManager.Instance.FirstSurfaceImage;
            _secondSurfaceImage = SceneManager.Instance.SecondSurfaceImage;
            _curvePoints = new List<Vector4>();
            BezierPatches = new[] { patches.ElementAt(0), patches.ElementAt(1) };
            _startPoints = new List<Point>();

            Color = Color.DeepPink;
        }
        #endregion Constructors
        #region Private Methods
        private void CalculateTrimmingCurve(Point mousePosition, Bitmap bmp, Graphics g, Graphics gf, Graphics gs)
        {
            var reverseTransform = SceneManager.Instance.TotalMatrix;
            reverseTransform.Invert();
            var pos = ModelTransform * reverseTransform * new Vector4(mousePosition.X, mousePosition.Y, 0, 1);
            var p = BezierPatches[0];
            var q = BezierPatches[1];
            int i, j, k, l;
            i = j = k = l = 0;
            Vector4 uvst = null, uvst1 = null;

            for (int iteration = 0; iteration < MaxTries; iteration++)
            {
                bool calculate = true;
                for (int pX = 0; pX < p.HorizontalPatches && calculate; pX++)
                    for (int pY = 0; pY < p.VerticalPatches && calculate; pY++)
                        for (int qX = 0; qX < q.HorizontalPatches && calculate; qX++)
                            for (int qY = 0; qY < q.VerticalPatches && calculate; qY++)
                            {
                                uvst = CalculateParametrization(pos, p, pY, pX, q, qY, qX);
                                uvst1 = CalculateParametrization(new Vector4(pos.X + NextPointDifference, pos.Y + NextPointDifference, pos.Z, pos.A), p, pY, pX, q, qY, qX);
                                if (uvst == null || uvst1 == null) continue;

                                if (uvst.X <= 1 && uvst.X >= 0 && uvst.Y <= 1 && uvst.Y >= 0 && uvst.Z <= 1 && uvst.Z >= 0 && uvst.A <= 1 && uvst.A >= 0
                                    && uvst1.X <= 1 && uvst1.X >= 0 && uvst1.Y <= 1 && uvst1.Y >= 0 && uvst1.Z <= 1 && uvst1.Z >= 0 && uvst1.A <= 1 && uvst1.A >= 0)
                                {
                                    j = pX; i = pY; l = qX; k = qY;
                                    calculate = false;
                                }
                            }
                if (uvst == null || uvst1 == null) pos = new Vector4(pos.X + SceneManager.Epsilon, pos.Y + SceneManager.Epsilon, pos.Z, pos.A);
            }

            if (uvst == null || uvst1 == null) return;

            var pointR = p.CalculatePatchPoint(i, j, uvst.X, uvst.Y);
            _curvePoints.Add(pointR);
            var pointR1 = p.CalculatePatchPoint(i, j, uvst1.X, uvst1.Y);
            _curvePoints.Add(pointR1);

            var pointL = pointR1;
            var pointL1 = pointR;
            int iL = i, iR = i, jL = j, jR = j, kL = k, kR = k, lL = l, lR = l;

            for (int w = 0; w < 1000; w++)
            {
                CalculateWholeCurve(bmp, g, gf, gs, ref pointR, ref pointR1, p, ref iR, ref jR, q, ref kR, ref lR);
                CalculateWholeCurve(bmp, g, gf, gs, ref pointL, ref pointL1, p, ref iL, ref jL, q, ref kL, ref lL);
            }
        }
        private void CalculateWholeCurve(Bitmap bmp, Graphics g, Graphics gf, Graphics gs, ref Vector4 point, ref Vector4 point1, BezierPatch p, ref int i, ref int j, BezierPatch q, ref int k, ref int l)
        {
            if (i < 0 || i >= p.VerticalPatches || j < 0 || j >= p.HorizontalPatches || k < 0 || k >= q.VerticalPatches || l < 0 || l >= q.HorizontalPatches) return;

            Vector4 uvst = null, nextPoint = null;
            var step = point1 - point;
            for (int iteration = 0; iteration < MaxTries && uvst == null; iteration++)
            {
                nextPoint = point1 + (step * (iteration + 1));
                uvst = CalculateParametrization(nextPoint, p, i, j, q, k, l);
            }

            if (uvst == null) return;
            if (uvst.X > 1 + SceneManager.Epsilon) { i++; uvst = new Vector4(uvst.X - 1, uvst.Y, uvst.Z, uvst.A); }
            if (uvst.X < -SceneManager.Epsilon) { i--; uvst = new Vector4(uvst.X + 1, uvst.Y, uvst.Z, uvst.A); }
            if (uvst.Y > 1 + SceneManager.Epsilon) { j++; uvst = new Vector4(uvst.X, uvst.Y - 1, uvst.Z, uvst.A); }
            if (uvst.Y < -SceneManager.Epsilon) { j--; uvst = new Vector4(uvst.X, uvst.Y + 1, uvst.Z, uvst.A); }
            if (uvst.Z > 1 + SceneManager.Epsilon) { k++; uvst = new Vector4(uvst.X, uvst.Y, uvst.Z - 1, uvst.A); }
            if (uvst.Z < -SceneManager.Epsilon) { k--; uvst = new Vector4(uvst.X, uvst.Y, uvst.Z + 1, uvst.A); }
            if (uvst.A > 1 + SceneManager.Epsilon) { l++; uvst = new Vector4(uvst.X, uvst.Y, uvst.Z, uvst.A - 1); }
            if (uvst.A < -SceneManager.Epsilon) { l--; uvst = new Vector4(uvst.X, uvst.Y, uvst.Z, uvst.A + 1); }
            if (i < 0 || i >= p.VerticalPatches || j < 0 || j >= p.HorizontalPatches || k < 0 || k >= q.VerticalPatches || l < 0 || l >= q.HorizontalPatches) return;

            var pt = p.CalculatePatchPoint(i, j, uvst.X, uvst.Y);
            _curvePoints.Add(pt);

            var fPt = new Vector4(uvst.X, uvst.Y, 0, 1);
            SceneManager.DrawPoint(_firstSurfaceImage, gf, fPt, Thickness, Color);
            var sPt = new Vector4(uvst.Z, uvst.A, 0, 1);
            SceneManager.DrawPoint(_secondSurfaceImage, gs, sPt, Thickness, Color);

            point = point1;
            point1 = nextPoint;
        }
        private Vector4 CalculateParametrization(Vector4 point, BezierPatch p, int i, int j, BezierPatch q, int k, int l)
        {
            var uv = NewtonRhapsonPointSurface(point, p, i, j);
            var st = NewtonRhapsonPointSurface(point, q, k, l);

            if (uv == null || st == null) return null;

            var uvst = NewtonRhapsonSurfaceSurface(p, uv.Y, uv.Z, i, j
                                                 , q, st.Y, st.Z, k, l);
            return uvst;
        }
        private Vector4 NewtonRhapsonPointSurface(Vector4 mousePosition, BezierPatch surface, int i, int j)
        {
            double epsilon = SceneManager.Epsilon;
            const int maxIterations = 20;

            double u = 0.5;
            double v = 0.5;
            Vector4 x1 = new Vector4(u, v, 0/*=t*/, 1);

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                var x = x1;
                var uPlus = surface.CalculatePatchPoint(i, j, x.Y + epsilon, x.Z);
                var uMinus = surface.CalculatePatchPoint(i, j, x.Y - epsilon, x.Z);
                var vPlus = surface.CalculatePatchPoint(i, j, x.Y, x.Z + epsilon);
                var vMinus = surface.CalculatePatchPoint(i, j, x.Y, x.Z - epsilon);
                var point = surface.CalculatePatchPoint(i, j, x.Y, x.Z);

                var jacobian = new[,]{{0.0, -(uPlus - uMinus).X/(2*epsilon), -(vPlus - vMinus).X/(2*epsilon)}
                                    , {0.0, -(uPlus - uMinus).Y/(2*epsilon), -(vPlus - vMinus).Y/(2*epsilon)}
                                    , {1.0, -(uPlus - uMinus).Z/(2*epsilon), -(vPlus - vMinus).Z/(2*epsilon)}};
                var inverse = jacobian.PseudoInverse();
                var error = new Vector4(mousePosition.X, mousePosition.Y, x.X, 1) - point;

                var dxByX = new Vector4(inverse[0, 0] * error.X + inverse[0, 1] * error.Y + inverse[0, 2] * error.Z
                                      , inverse[1, 0] * error.X + inverse[1, 1] * error.Y + inverse[1, 2] * error.Z
                                      , inverse[2, 0] * error.X + inverse[2, 1] * error.Y + inverse[2, 2] * error.Z
                                      , 1);
                x1 = x - dxByX;
                if (Math.Abs((x1 - x).X) + Math.Abs((x1 - x).Y) + Math.Abs((x1 - x).Z) < epsilon)
                    return x1;
            }
            return null;
        }
        private Vector4 NewtonRhapsonSurfaceSurface(BezierPatch p, double u, double v, int i, int j, BezierPatch q, double s, double t, int k, int l)
        {
            double epsilon = SceneManager.Epsilon;
            const int maxIterations = 20;

            Vector4 x1 = new Vector4(u, v, s, t);

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                var x = x1;
                var uPlus = p.CalculatePatchPoint(i, j, x.X + epsilon, x.Y);
                var uMinus = p.CalculatePatchPoint(i, j, x.X - epsilon, x.Y);
                var vPlus = p.CalculatePatchPoint(i, j, x.X, x.Y + epsilon);
                var vMinus = p.CalculatePatchPoint(i, j, x.X, x.Y - epsilon);
                var uvPoint = p.CalculatePatchPoint(i, j, x.X, x.Y);

                var sPlus = q.CalculatePatchPoint(k, l, x.Z + epsilon, x.A);
                var sMinus = q.CalculatePatchPoint(k, l, x.Z - epsilon, x.A);
                var tPlus = q.CalculatePatchPoint(k, l, x.Z, x.A + epsilon);
                var tMinus = q.CalculatePatchPoint(k, l, x.Z, x.A - epsilon);
                var stPoint = q.CalculatePatchPoint(k, l, x.Z, x.A);

                var col0 = new Vector4((uPlus - uMinus).X / (2 * epsilon), (uPlus - uMinus).Y / (2 * epsilon), (uPlus - uMinus).Z / (2 * epsilon), 1);
                var col1 = new Vector4((vPlus - vMinus).X / (2 * epsilon), (vPlus - vMinus).Y / (2 * epsilon), (vPlus - vMinus).Z / (2 * epsilon), 1);
                var uvNormal = ((uPlus - uMinus) / (2 * epsilon)).Cross((vPlus - vMinus) / (2 * epsilon));
                var stNormal = ((sPlus - sMinus) / (2 * epsilon)).Cross((tPlus - tMinus) / (2 * epsilon));
                var normal = uvNormal.Cross(stNormal);
                var totalNormal = new Vector4(normal.X, normal.Y, normal.Z, 0).Normalized;

                var jacobian = new[,]{{(uPlus - uMinus).X/(2*epsilon), (vPlus - vMinus).X/(2*epsilon), -(sPlus - sMinus).X/(2*epsilon), -(tPlus - tMinus).X/(2*epsilon)}
                                    , {(uPlus - uMinus).Y/(2*epsilon), (vPlus - vMinus).Y/(2*epsilon), -(sPlus - sMinus).Y/(2*epsilon), -(tPlus - tMinus).Y/(2*epsilon)}
                                    , {(uPlus - uMinus).Z/(2*epsilon), (vPlus - vMinus).Z/(2*epsilon), -(sPlus - sMinus).Z/(2*epsilon), -(tPlus - tMinus).Z/(2*epsilon)}
                                    , {col0.Dot(totalNormal), col1.Dot(totalNormal), 0, 0}};
                var inverse = jacobian.PseudoInverse();
                var error = uvPoint - stPoint;

                var dxByX = new Vector4(inverse[0, 0] * error.X + inverse[0, 1] * error.Y + inverse[0, 2] * error.Z + inverse[0, 3] * error.A
                                      , inverse[1, 0] * error.X + inverse[1, 1] * error.Y + inverse[1, 2] * error.Z + inverse[1, 3] * error.A
                                      , inverse[2, 0] * error.X + inverse[2, 1] * error.Y + inverse[2, 2] * error.Z + inverse[2, 3] * error.A
                                      , inverse[3, 0] * error.X + inverse[3, 1] * error.Y + inverse[3, 2] * error.Z + inverse[3, 3] * error.A);
                x1 = x - dxByX;
                if (Math.Abs((x1 - x).X) + Math.Abs((x1 - x).Y) + Math.Abs((x1 - x).Z) + Math.Abs((x1 - x).A) < epsilon)
                    return x1;
            }
            return null;
        }
        #endregion Private Methods
        #region Public Methods
        public override void Draw()
        {
            Bitmap bmp = SceneManager.Instance.SceneImage;
            using (Graphics g = Graphics.FromImage(bmp))
            {
                foreach (var point in _curvePoints)
                    SceneManager.DrawPoint(bmp, g, point, Thickness, Color);
            }
            SceneManager.Instance.SceneImage = bmp;
        }
        public bool TrimCurve()
        {
            Bitmap bmp = SceneManager.Instance.SceneImage;
            using (Graphics g = Graphics.FromImage(bmp))
            {
                using (Graphics gf = Graphics.FromImage(_firstSurfaceImage))
                {
                    gf.Clear(Color.Black);
                    using (Graphics gs = Graphics.FromImage(_secondSurfaceImage))
                    {
                        gs.Clear(Color.Black);
                        foreach (var point in StartPoints)
                            CalculateTrimmingCurve(point, bmp, g, gf, gs);
                    }
                }
            }
            SceneManager.Instance.FirstSurfaceImage = _firstSurfaceImage;
            SceneManager.Instance.SecondSurfaceImage = _secondSurfaceImage;
            SceneManager.Instance.SceneImage = bmp;
            Draw();
            return true;
        }
        #endregion Public Methods
    }
}

