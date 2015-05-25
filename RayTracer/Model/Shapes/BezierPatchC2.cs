using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Media.Media3D;
using RayTracer.Helpers;
using RayTracer.ViewModel;

namespace RayTracer.Model.Shapes
{
    public class BezierPatchC2 : BezierPatch
    {
        #region Private Members
        private double[] _knotsVertical;
        private double[] _knotsHorizontal;
        #endregion Private Members
        #region Public Properties
        public override string Type { get { return "BezierSurfaceC2"; } }
        #endregion Public Properties
        #region Constructors
        public BezierPatchC2(double x, double y, double z, string name, bool isCylinder, double width, double height
            , int verticalPatches, int horizontalPatches, PointEx[,] points = null, IEnumerable<PointEx> vertices = null)
            : base(x, y, z, name, isCylinder, width, height, verticalPatches, horizontalPatches, points, vertices)
        {
            const int n = SceneManager.BezierSegmentPoints;
            var manager = PatchManager.Instance;
            SetVertices(points, vertices, SceneManager.BezierSegmentPoints + manager.VerticalPatches, SceneManager.BezierSegmentPoints + manager.HorizontalPatches);
            SetSplineKnots(n);
        }
        #endregion Constructors
        #region Private Methods
        private void SetSplineKnots(int n)
        {
            _knotsVertical = new double[(VerticalPatches * n + 1) + n + 4];
            _knotsHorizontal = new double[(HorizontalPatches * n + 1) + n + 4];

            double interval = 1 / (double)(_knotsVertical.Length - 1);
            for (int i = 0; i < _knotsVertical.Length; i++)
                _knotsVertical[i] = i * interval;

            interval = 1 / (double)(_knotsHorizontal.Length - 1);
            for (int i = 0; i < _knotsHorizontal.Length; i++)
                _knotsHorizontal[i] = i * interval;
        }
        private void DrawSinglePatch(Bitmap bmp, Graphics g, int patchDivisions, Vector4[,] points, int divisions, bool isHorizontal)
        {
            double step = 1.0f / (patchDivisions - 1);
            double drawingStep = 1.0f / (divisions - 1);
            double u = 0;
            double v = 0;
            var uArray = isHorizontal ? _knotsHorizontal : _knotsVertical;
            var vArray = isHorizontal ? _knotsVertical : _knotsHorizontal;

            for (int m = 0; m <= patchDivisions + SceneManager.BezierSegmentPoints + 4; m++, u += step)
            {
                if (u < uArray[3] || u > uArray[uArray.Length - SceneManager.BezierSegmentPoints - 4]) continue;
                double[] nu = InitializeNArray(u, uArray);

                for (double n = 0; n <= divisions + SceneManager.BezierSegmentPoints + 4; n++, v += drawingStep)
                {
                    if (v < vArray[3] || v > vArray[vArray.Length - SceneManager.BezierSegmentPoints - 4]) continue;
                    double[] nv = InitializeNArray(v, vArray);

                    Vector4 value;
                    if (isHorizontal) value = CalculatePatchValue(points, nv, nu);
                    else value = CalculatePatchValue(points, nu, nv);
                    SceneManager.DrawCurvePoint(bmp, g, value, Thickness);
                }
            }
        }
        private Vector4 CalculatePatchValue(Vector4[,] points, double[] nu, double[] nv)
        {
            var point = new Vector4(0, 0, 0, 1);

            for (int i = 0; i < SceneManager.BezierSegmentPoints + 1; i++)
                for (int j = 0; j < SceneManager.BezierSegmentPoints + 1; j++)
                    point += points[j, i] * nu[j] * nv[i];

            return new Vector4(point.X, point.Y, point.Z, 1);
        }
        private double[] InitializeNArray(double u, double[] knots)
        {
            var n = new double[4];
            for (int i = 0; i < SceneManager.BezierSegmentPoints + 1; i++)
                n[i] = knots.GetNFunctionValue(i, SceneManager.BezierSegmentPoints, u);
            return n;
        }
        #endregion Private Methods
        #region Public Methods
        public override void Draw()
        {
            base.Draw();
            var manager = PatchManager.Instance;
            const int bezierSegmentPoints = SceneManager.BezierSegmentPoints;
            double maxX, maxY, minX, minY;
            Points.FindMaxMinCoords(out minX, out minY, out maxX, out maxY);

            var xDiv = (maxX - minX) * 4;
            var yDiv = (maxY - minY) * 4;

            Bitmap bmp = SceneManager.Instance.SceneImage;
            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < VerticalPatches; i++)
                    for (int j = 0; j < HorizontalPatches; j++)
                    {
                        var points = new[,]{{Points[i + 0, j ].TransformedPosition, Points[i + 0, j + 1].TransformedPosition, Points[i + 0, j + 2].TransformedPosition, Points[i + 0, j + 3].TransformedPosition}
                                            ,{ Points[i + 1, j ].TransformedPosition, Points[i + 1, j + 1].TransformedPosition, Points[i + 1, j + 2].TransformedPosition, Points[i + 1, j + 3].TransformedPosition}
                                            ,{ Points[i + 2, j ].TransformedPosition, Points[i + 2, j + 1].TransformedPosition, Points[i + 2, j + 2].TransformedPosition, Points[i + 2, j + 3].TransformedPosition}
                                            , {Points[i + 3, j ].TransformedPosition, Points[i + 3, j + 1].TransformedPosition, Points[i + 3, j + 2].TransformedPosition, Points[i + 3, j + 3].TransformedPosition}};

                        DrawSinglePatch(bmp, g, manager.VerticalPatchDivisions, points, (int)Math.Max(xDiv, yDiv), isHorizontal: false);
                        DrawSinglePatch(bmp, g, manager.HorizontalPatchDivisions, points, (int)Math.Max(xDiv, yDiv), isHorizontal: true);
                    }
            }
            SceneManager.Instance.SceneImage = bmp;
        }
        #endregion Public Methods
    }
}

