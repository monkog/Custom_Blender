using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media.Media3D;
using RayTracer.Helpers;
using RayTracer.ViewModel;

namespace RayTracer.Model.Shapes
{
    public class BezierPatchC0 : BezierPatch
    {

        #region Public Properties
        public override string Type { get { return "BezierSurfaceC0"; } }
        #endregion Public Properties
        #region Constructors
        public BezierPatchC0(double x, double y, double z, string name, bool isCylinder, double width, double height
            , int verticalPatches, int horizontalPatches, PointEx[,] points = null, IEnumerable<PointEx> vertices = null)
            : base(x, y, z, name, isCylinder, width, height, verticalPatches, horizontalPatches, points, vertices)
        {        }
        #endregion Constructors
        #region Protected Methods
        protected override void DrawSinglePatch(Bitmap bmp, Graphics g, int patchIndex, int patchDivisions, Matrix3D matX, Matrix3D matY, Matrix3D matZ
            , double divisions, bool isHorizontal)
        {
            double step = 1.0f / (patchDivisions - 1);
            double currentStep = patchIndex == 0 ? 0 : step;
            Vector4 pointX = null, pointY = null;

            for (double m = (patchIndex == 0 ? 0 : 1); m < patchDivisions; m++, currentStep += step)
            {
                if (isHorizontal)
                    pointY = new Vector4(Math.Pow((1.0 - currentStep), 3), 3 * currentStep * Math.Pow((1.0 - currentStep), 2), 3 * currentStep * currentStep * (1.0 - currentStep), Math.Pow(currentStep, 3));
                else
                    pointX = new Vector4(Math.Pow((1.0 - currentStep), 3), 3 * currentStep * Math.Pow((1.0 - currentStep), 2), 3 * currentStep * currentStep * (1.0 - currentStep), Math.Pow(currentStep, 3));

                for (double n = 0; n <= 1; n += divisions)
                {
                    if (isHorizontal)
                        pointX = new Vector4(Math.Pow((1.0 - n), 3), 3 * n * Math.Pow((1.0 - n), 2), 3 * n * n * (1.0 - n), Math.Pow(n, 3));
                    else
                        pointY = new Vector4(Math.Pow((1.0 - n), 3), 3 * n * Math.Pow((1.0 - n), 2), 3 * n * n * (1.0 - n), Math.Pow(n, 3));

                    var x = pointX * matX * pointY;
                    var y = pointX * matY * pointY;
                    var z = pointX * matZ * pointY;
                    SceneManager.DrawCurvePoint(bmp, g, new Vector4(x, y, z, 1), Thickness);
                }
            }
        }
        #endregion Protected Methods
        #region Public Methods
        public override void Draw()
        {
            base.Draw();
            var manager = PatchManager.Instance;
            const int bezierSegmentPoints = SceneManager.BezierSegmentPoints;
            double maxX, maxY, minX, minY;
            Points.FindMaxMinCoords(out minX, out minY, out maxX, out maxY);

            var xDiv = 1.0 / ((maxX - minX) * 4);
            var yDiv = 1.0 / ((maxY - minY) * 4);

            Bitmap bmp = SceneManager.Instance.SceneImage;
            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < VerticalPatches; i++)
                {
                    for (int j = 0; j < HorizontalPatches; j++)
                    {
                        Matrix3D matX = new Matrix3D(Points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints].TransformedPosition.X, Points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 1].TransformedPosition.X, Points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 2].TransformedPosition.X, Points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 3].TransformedPosition.X
                                                   , Points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints].TransformedPosition.X, Points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 1].TransformedPosition.X, Points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 2].TransformedPosition.X, Points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 3].TransformedPosition.X
                                                   , Points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints].TransformedPosition.X, Points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 1].TransformedPosition.X, Points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 2].TransformedPosition.X, Points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 3].TransformedPosition.X
                                                   , Points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints].TransformedPosition.X, Points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 1].TransformedPosition.X, Points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 2].TransformedPosition.X, Points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 3].TransformedPosition.X);
                        Matrix3D matY = new Matrix3D(Points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints].TransformedPosition.Y, Points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 1].TransformedPosition.Y, Points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 2].TransformedPosition.Y, Points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 3].TransformedPosition.Y
                                                   , Points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints].TransformedPosition.Y, Points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 1].TransformedPosition.Y, Points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 2].TransformedPosition.Y, Points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 3].TransformedPosition.Y
                                                   , Points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints].TransformedPosition.Y, Points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 1].TransformedPosition.Y, Points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 2].TransformedPosition.Y, Points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 3].TransformedPosition.Y
                                                   , Points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints].TransformedPosition.Y, Points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 1].TransformedPosition.Y, Points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 2].TransformedPosition.Y, Points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 3].TransformedPosition.Y);
                        Matrix3D matZ = new Matrix3D(Points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints].TransformedPosition.Z, Points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 1].TransformedPosition.Z, Points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 2].TransformedPosition.Z, Points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 3].TransformedPosition.Z
                                                   , Points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints].TransformedPosition.Z, Points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 1].TransformedPosition.Z, Points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 2].TransformedPosition.Z, Points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 3].TransformedPosition.Z
                                                   , Points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints].TransformedPosition.Z, Points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 1].TransformedPosition.Z, Points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 2].TransformedPosition.Z, Points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 3].TransformedPosition.Z
                                                   , Points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints].TransformedPosition.Z, Points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 1].TransformedPosition.Z, Points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 2].TransformedPosition.Z, Points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 3].TransformedPosition.Z);

                        DrawSinglePatch(bmp, g, i, manager.VerticalPatchDivisions, matX, matY, matZ, Math.Max(xDiv, yDiv), isHorizontal: false);
                        DrawSinglePatch(bmp, g, j, manager.HorizontalPatchDivisions, matX, matY, matZ, Math.Max(xDiv, yDiv), isHorizontal: true);
                    }
                }
            }
            SceneManager.Instance.SceneImage = bmp;
        }
        #endregion Public Methods
    }
}

