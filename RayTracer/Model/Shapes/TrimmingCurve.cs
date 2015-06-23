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
        private Point _startPoint;
        Bitmap _firstSurfaceImage;
        Bitmap _secondSurfaceImage;
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
        /// Gets or sets the start point for the Newton's method.
        /// </summary>
        public Point StartPoint
        {
            get { return _startPoint; }
            set
            {
                if (_startPoint == value) return;
                _startPoint = value;
                OnPropertyChanged("StartPoint");
            }
        }
        #endregion Public Properties
        #region Constructors
        public TrimmingCurve(double x, double y, double z, string name, List<BezierPatch> patches)
            : base(x, y, z, name)
        {
            _firstSurfaceImage = SceneManager.Instance.FirstSurfaceImage;
            _secondSurfaceImage = SceneManager.Instance.SecondSurfaceImage;
            MouseEventManager.Instance.CaptureNewtonStartPoint = true;
            BezierPatches = new[] { patches.ElementAt(0), patches.ElementAt(1) };

            Color = Color.Pink;
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

            var uvst = CalculateParametrization(pos, p, i, j, q, k, l);
            var uvst1 = CalculateParametrization(new Vector4(pos.X +0.01, pos.Y + 0.01, pos.Z, pos.A), p, i, j, q, k, l);

            if (uvst == null || uvst1 == null) return;

            var pointR = p.CalculatePatchPoint(i, j, uvst.X, uvst.Y);
            SceneManager.DrawPoint(bmp, g, pointR, Thickness, Color);
            var pointR1 = p.CalculatePatchPoint(i, j, uvst1.X, uvst1.Y);
            SceneManager.DrawPoint(bmp, g, pointR1, Thickness, Color);

            var pointL = pointR1;
            var pointL1 = pointR;

            for (int w = 0; w < 1000; w++)
            {
                CalculateWholeCurve(bmp, g, gf, gs, ref pointR, ref pointR1, p, i, j, q, k, l);
                CalculateWholeCurve(bmp, g, gf, gs, ref pointL, ref pointL1, p, i, j, q, k, l);
            }
        }
        private void CalculateWholeCurve(Bitmap bmp, Graphics g, Graphics gf, Graphics gs, ref Vector4 point, ref Vector4 point1, BezierPatch p, int i, int j, BezierPatch q, int k, int l)
        {
            var nextPoint = point1 * 2 - point;
            var uvst = CalculateParametrization(nextPoint, p, i, j, q, k, l);

            if (uvst == null || uvst.X > 1 || uvst.X < 0 || uvst.Y > 1 || uvst.Y < 0 || uvst.Z > 1 || uvst.Z < 0 || uvst.A > 1 || uvst.A < 0) return;

            var pt = p.CalculatePatchPoint(i, j, uvst.X, uvst.Y);
            SceneManager.DrawPoint(bmp, g, pt, Thickness, Color);

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

                var sPlus = q.CalculatePatchPoint(i, j, x.Z + epsilon, x.A);
                var sMinus = q.CalculatePatchPoint(i, j, x.Z - epsilon, x.A);
                var tPlus = q.CalculatePatchPoint(i, j, x.Z, x.A + epsilon);
                var tMinus = q.CalculatePatchPoint(i, j, x.Z, x.A - epsilon);
                var stPoint = q.CalculatePatchPoint(i, j, x.Z, x.A);

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
                using (Graphics gf = Graphics.FromImage(_firstSurfaceImage))
                {
                    gf.Clear(Color.Black);
                    using (Graphics gs = Graphics.FromImage(_secondSurfaceImage))
                    {
                        gs.Clear(Color.Black);
                        CalculateTrimmingCurve(StartPoint, bmp, g, gf, gs);
                    }
                }
            }
            SceneManager.Instance.FirstSurfaceImage = _firstSurfaceImage;
            SceneManager.Instance.SecondSurfaceImage = _secondSurfaceImage;
            SceneManager.Instance.SceneImage = bmp;
        }
        #endregion Public Methods
    }
}

