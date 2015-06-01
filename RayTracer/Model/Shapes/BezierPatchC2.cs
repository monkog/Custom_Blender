using System;
using System.Collections.Generic;
using System.Drawing;
using RayTracer.Helpers;
using RayTracer.ViewModel;

namespace RayTracer.Model.Shapes
{
    public class BezierPatchC2 : BezierPatch
    {
        #region Private Members
        private double[] _knots;
        #endregion Private Members
        #region Public Properties
        public override string Type { get { return "BezierSurfaceC2"; } }
        #endregion Public Properties
        #region Constructors
        public BezierPatchC2(double x, double y, double z, string name, bool isCylinder, double width, double height
            , int verticalPatches, int horizontalPatches, PointEx[,] points = null, IEnumerable<PointEx> vertices = null)
            : base(x, y, z, name, isCylinder, width, height, verticalPatches, horizontalPatches)
        {
            Continuity = Continuity.C2;
            var manager = PatchManager.Instance;
            SetVertices(points, vertices, SceneManager.BezierSegmentPoints + manager.VerticalPatches, SceneManager.BezierSegmentPoints + manager.HorizontalPatches);
            SetSplineKnots();
        }
        #endregion Constructors
        #region Private Methods
        private void SetSplineKnots()
        {
            _knots = new double[2 * SceneManager.BezierSegmentPoints + 2];
            _knots[0] = 0;
            for (int i = 0; i <= SceneManager.BezierSegmentPoints + 2; i++)
                _knots[i + 1] = i;
            _knots[2 * SceneManager.BezierSegmentPoints + 1] = SceneManager.BezierSegmentPoints + 2;
        }
        private void DrawSinglePatch(Bitmap bmp, Graphics g, int patchDivisions, Vector4[,] points, int divisions, bool isHorizontal)
        {
            double step = 1.0f / (patchDivisions - 1);
            double drawingStep = 1.0f / (divisions - 1);
            double u = 0;
            double[] uArray = null;
            double[] vArray = null;

            for (int m = 0; m < patchDivisions; m++, u += step)
            {
                if (isHorizontal) uArray = InitializeNArray(2 + u, _knots);
                else vArray = InitializeNArray(2 + u, _knots);
                double v = 0;

                for (double n = 0; n < divisions; n++, v += drawingStep)
                {
                    if (isHorizontal) vArray = InitializeNArray(2 + v, _knots);
                    else uArray = InitializeNArray(2 + v, _knots);

                    Vector4 value = CalculatePatchValue(points, uArray, vArray);
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
            double maxX, maxY, minX, minY;
            Points.FindMaxMinCoords(out minX, out minY, out maxX, out maxY);

            var xDiv = Math.Min(100, (maxX - minX) * 4);
            var yDiv = Math.Min(100, (maxY - minY) * 4);

            Bitmap bmp = SceneManager.Instance.SceneImage;
            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < VerticalPatches; i++)
                    for (int j = 0; j < HorizontalPatches; j++)
                    {
                        var points = new[,]{ { Points[i + 0, j ].TransformedPosition, Points[i + 0, (j + 1) % Points.GetLength(1)].TransformedPosition, Points[i + 0, (j + 2) % Points.GetLength(1)].TransformedPosition, Points[i + 0, (j + 3) % Points.GetLength(1)].TransformedPosition}
                                            ,{ Points[i + 1, j ].TransformedPosition, Points[i + 1, (j + 1) % Points.GetLength(1)].TransformedPosition, Points[i + 1, (j + 2) % Points.GetLength(1)].TransformedPosition, Points[i + 1, (j + 3) % Points.GetLength(1)].TransformedPosition}
                                            ,{ Points[i + 2, j ].TransformedPosition, Points[i + 2, (j + 1) % Points.GetLength(1)].TransformedPosition, Points[i + 2, (j + 2) % Points.GetLength(1)].TransformedPosition, Points[i + 2, (j + 3) % Points.GetLength(1)].TransformedPosition}
                                            ,{ Points[i + 3, j ].TransformedPosition, Points[i + 3, (j + 1) % Points.GetLength(1)].TransformedPosition, Points[i + 3, (j + 2) % Points.GetLength(1)].TransformedPosition, Points[i + 3, (j + 3) % Points.GetLength(1)].TransformedPosition}};

                        DrawSinglePatch(bmp, g, manager.VerticalPatchDivisions, points, (int)Math.Max(xDiv, yDiv), isHorizontal: false);
                        DrawSinglePatch(bmp, g, manager.HorizontalPatchDivisions, points, (int)Math.Max(xDiv, yDiv), isHorizontal: true);
                    }
            }
            SceneManager.Instance.SceneImage = bmp;
        }
        #endregion Public Methods
    }
}

