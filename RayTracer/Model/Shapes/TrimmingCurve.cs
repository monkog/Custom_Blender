using System;
using System.Collections.Generic;
using System.Linq;
using RayTracer.Helpers;
using RayTracer.ViewModel;
using Accord.Math;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
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
        private bool CalculateTrimmingCurve(Point mousePosition, Graphics g, Graphics gf, Graphics gs)
        {
            var reverseTransform = SceneManager.Instance.TotalMatrix;
            reverseTransform.Invert();
            var pos = ModelTransform * reverseTransform * new Vector4(mousePosition.X, mousePosition.Y, 0, 1);
            var p = BezierPatches[0];
            var q = BezierPatches[1];
            int i = 0, j = 0, k = 0, l = 0;
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
            if (uvst == null || uvst1 == null) return false;

            _curvePoints.Add(p.CalculatePatchPoint(i, j, uvst.X, uvst.Y));
            _curvePoints.Add(p.CalculatePatchPoint(i, j, uvst1.X, uvst1.Y));

            var pointL = new Vector4(uvst.X, uvst.Y, uvst.Z, uvst.A);
            var pointR = pointL;
            int iL = i, iR = i, jL = j, jR = j, kL = k, kR = k, lL = l, lR = l;

            while (CalculateWholeCurve(g, gf, gs, ref pointL, p, ref iL, ref jL, q, ref kL, ref lL)) ;
            _curvePoints.Reverse();
            while (CalculateWholeCurve(g, gf, gs, ref pointR, p, ref iR, ref jR, q, ref kR, ref lR, isOtherWay: true)) ;
            return true;
        }
        private bool CalculateWholeCurve(Graphics g, Graphics gf, Graphics gs, ref Vector4 point, BezierPatch p, ref int i, ref int j, BezierPatch q, ref int k, ref int l, bool isOtherWay = false)
        {
            if (i < 0 || i >= p.VerticalPatches || j < 0 || j >= p.HorizontalPatches || k < 0 || k >= q.VerticalPatches || l < 0 || l >= q.HorizontalPatches) return false;

            Vector4 uvst = null, nextPoint = null;
            for (int iteration = 0; iteration < MaxTries && uvst == null; iteration++)
            {
                nextPoint = GetNextPoint(point, p, i, j, q, k, l, iteration, isOtherWay);
                uvst = NewtonRhapsonSurfaceSurface(p, nextPoint.X, nextPoint.Y, i, j, q, nextPoint.Z, nextPoint.A, k, l);
            }

            if (uvst == null)
            {
                point = nextPoint;
                return true;
            }

            if (uvst.X > 1 + SceneManager.Epsilon) { i++; uvst = new Vector4(uvst.X - 1, uvst.Y, uvst.Z, uvst.A); }
            if (uvst.X < -SceneManager.Epsilon) { i--; uvst = new Vector4(uvst.X + 1, uvst.Y, uvst.Z, uvst.A); }
            if (uvst.Y > 1 + SceneManager.Epsilon) { j++; uvst = new Vector4(uvst.X, uvst.Y - 1, uvst.Z, uvst.A); }
            if (uvst.Y < -SceneManager.Epsilon) { j--; uvst = new Vector4(uvst.X, uvst.Y + 1, uvst.Z, uvst.A); }
            if (uvst.Z > 1 + SceneManager.Epsilon) { k++; uvst = new Vector4(uvst.X, uvst.Y, uvst.Z - 1, uvst.A); }
            if (uvst.Z < -SceneManager.Epsilon) { k--; uvst = new Vector4(uvst.X, uvst.Y, uvst.Z + 1, uvst.A); }
            if (uvst.A > 1 + SceneManager.Epsilon) { l++; uvst = new Vector4(uvst.X, uvst.Y, uvst.Z, uvst.A - 1); }
            if (uvst.A < -SceneManager.Epsilon) { l--; uvst = new Vector4(uvst.X, uvst.Y, uvst.Z, uvst.A + 1); }
            if (i < 0 || i >= p.VerticalPatches || j < 0 || j >= p.HorizontalPatches || k < 0 || k >= q.VerticalPatches || l < 0 || l >= q.HorizontalPatches) return false;

            var pt = p.CalculatePatchPoint(i, j, uvst.X, uvst.Y);
            _curvePoints.Add(pt);
            var po = SceneManager.Instance.TransformMatrix * SceneManager.Instance.ScaleMatrix * Transformations.ViewMatrix(400) *
                    pt;
            g.DrawRectangle(new Pen(new SolidBrush(Color)), new Rectangle((int)po.X, (int)po.Y, 2, 2));

            var fPt = new Vector4(uvst.X, uvst.Y, 0, 1);
            SceneManager.DrawPoint(_firstSurfaceImage, gf, fPt, Thickness, Color);
            var sPt = new Vector4(uvst.Z, uvst.A, 0, 1);
            SceneManager.DrawPoint(_secondSurfaceImage, gs, sPt, Thickness, Color);

            point = uvst;
            return true;
        }
        private Vector4 GetNextPoint(Vector4 point, BezierPatch p, int i, int j, BezierPatch q, int k, int l, int iteration, bool isOtherWay)
        {
            var dpdu = p.CalculatePatchPoint(i, j, point.X, point.Y, uDerivative: true);
            var dpdv = p.CalculatePatchPoint(i, j, point.X, point.Y, uDerivative: false, vDerivative: true);
            var dqdu = q.CalculatePatchPoint(k, l, point.X, point.Y, uDerivative: true) * (-1);
            var dqdv = q.CalculatePatchPoint(k, l, point.X, point.Y, uDerivative: false, vDerivative: true) * (-1);

            var pCross = dpdu.Cross(dpdv);
            var qCross = dqdu.Cross(dqdv);
            return point + ((pCross.Cross(qCross).Normalized / (iteration * 30.0)) * (isOtherWay ? -1 : 1));
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
            const int maxIterations = 20;

            double u = 0.5;
            double v = 0.5;
            Vector4 x1 = new Vector4(u, v, 0/*=t*/, 1);

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                var x = x1;
                var uPrim = surface.CalculatePatchPoint(i, j, x.Y, x.Z, uDerivative: true).Normalized;
                var vPrim = surface.CalculatePatchPoint(i, j, x.Y, x.Z, uDerivative: false, vDerivative: true).Normalized;
                var point = surface.CalculatePatchPoint(i, j, x.Y, x.Z);

                var jacobian = new[,]{{0.0, -uPrim.X, -vPrim.X}
                                    , {0.0, -uPrim.Y, -vPrim.Y}
                                    , {1.0, -uPrim.Z, -vPrim.Z}};
                var inverse = jacobian.PseudoInverse();
                var error = new Vector4(mousePosition.X, mousePosition.Y, x.X, 1) - point;

                var dxByX = new Vector4(inverse[0, 0] * error.X + inverse[0, 1] * error.Y + inverse[0, 2] * error.Z
                                      , inverse[1, 0] * error.X + inverse[1, 1] * error.Y + inverse[1, 2] * error.Z
                                      , inverse[2, 0] * error.X + inverse[2, 1] * error.Y + inverse[2, 2] * error.Z
                                      , 1);
                x1 = x - dxByX;
                if (Math.Abs((x1 - x).X) + Math.Abs((x1 - x).Y) + Math.Abs((x1 - x).Z) < SceneManager.Epsilon)
                    return x1;
            }
            return null;
        }
        private Vector4 NewtonRhapsonSurfaceSurface(BezierPatch p, double u, double v, int i, int j, BezierPatch q, double s, double t, int k, int l)
        {
            const int maxIterations = 20;

            Vector4 x1 = new Vector4(u, v, s, t);

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                var x = x1;
                var uPrim = p.CalculatePatchPoint(i, j, x.Y, x.Z, uDerivative: true).Normalized;
                var vPrim = p.CalculatePatchPoint(i, j, x.Y, x.Z, uDerivative: false, vDerivative: true).Normalized;
                var uvPoint = p.CalculatePatchPoint(i, j, x.X, x.Y);

                var sPrim = q.CalculatePatchPoint(k, l, x.Z, x.A, uDerivative: true).Normalized;
                var tPrim = q.CalculatePatchPoint(k, l, x.Z, x.A, uDerivative: false, vDerivative: true).Normalized;
                var stPoint = q.CalculatePatchPoint(k, l, x.Z, x.A);

                var jacobian = new[,]{{uPrim.X, vPrim.X, -sPrim.X, -tPrim.X}
                                    , {uPrim.Y, vPrim.Y, -sPrim.Y, -tPrim.Y}
                                    , {uPrim.Z, vPrim.Z, -sPrim.Z, -tPrim.Z}
                                    , {0, 0, 0, 1}};
                var inverse = jacobian.PseudoInverse();
                var error = uvPoint - stPoint;

                var dxByX = new Vector4(inverse[0, 0] * error.X + inverse[0, 1] * error.Y + inverse[0, 2] * error.Z + inverse[0, 3] * error.A
                                      , inverse[1, 0] * error.X + inverse[1, 1] * error.Y + inverse[1, 2] * error.Z + inverse[1, 3] * error.A
                                      , inverse[2, 0] * error.X + inverse[2, 1] * error.Y + inverse[2, 2] * error.Z + inverse[2, 3] * error.A
                                      , inverse[3, 0] * error.X + inverse[3, 1] * error.Y + inverse[3, 2] * error.Z + inverse[3, 3] * error.A);
                x1 = x - dxByX;
                if (Math.Abs((x1 - x).X) + Math.Abs((x1 - x).Y) + Math.Abs((x1 - x).Z) + Math.Abs((x1 - x).A) < SceneManager.Epsilon)
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
                //foreach (var point in _curvePoints)
                //    SceneManager.DrawPoint(bmp, g, point, Thickness, Color);
                for (int i = 0; i < _curvePoints.Count - 1; i++)
                    SceneManager.DrawLine(bmp, g, _curvePoints[i], _curvePoints[i + 1], Thickness, Color);
            }
            SceneManager.Instance.SceneImage = bmp;
        }

        public bool TrimCurve()
        {
            _curvePoints.Clear();
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
                            if (!CalculateTrimmingCurve(point, g, gf, gs))
                                MessageBox.Show("Could not find parametrization");
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
        #region Commands
        private ICommand _calculateCommand;
        public ICommand CalculateCommand { get { return _calculateCommand ?? (_calculateCommand = new DelegateCommand(CalculateeExecuted)); } }
        /// <summary>
        /// Trimms the surfaces.
        /// </summary>
        private void CalculateeExecuted()
        {
            MouseEventManager.Instance.CaptureNewtonStartPoint = true;
        }
        #endregion Commands
    }
}

