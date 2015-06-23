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
            : base(x, y, z, name, isCylinder, width, height, verticalPatches, horizontalPatches)
        {
            var manager = PatchManager.Instance;
            SetVertices(points, vertices, manager.VerticalPatches * SceneManager.BezierSegmentPoints + 1, manager.HorizontalPatches * SceneManager.BezierSegmentPoints + 1);
            Continuity = Continuity.C0;
            DisplayEdges = false;
        }
        #endregion Constructors
        #region Private Methods
        public Vector4 CalculatePatchPoint(Matrix3D matX, Matrix3D matY, Matrix3D matZ, Vector4 pointX, Vector4 pointY)
        {
            var x = pointX * matX * pointY;
            var y = pointX * matY * pointY;
            var z = pointX * matZ * pointY;
            var point = new Vector4(x, y, z, 1);
            return point;
        }
        #endregion Private Methods
        #region Protected Methods
        protected void DrawSinglePatch(Bitmap bmp, Graphics g, int patchIndex, int patchDivisions, Matrix3D matX, Matrix3D matY, Matrix3D matZ
            , int divisions, bool isHorizontal)
        {
            double step = 1.0f / (patchDivisions - 1);
            double drawingStep = 1.0f / (divisions - 1);
            double u = patchIndex == 0 ? 0 : step;
            Vector4 pointX = null, pointY = null;

            for (int m = (patchIndex == 0 ? 0 : 1); m < patchDivisions; m++, u += step)
            {
                if (isHorizontal)
                    pointY = u.GetBezierPoint();
                else
                    pointX = u.GetBezierPoint();

                for (int n = 0; n < divisions; n++)
                {
                    var v = n * drawingStep;
                    if (isHorizontal)
                        pointX = v.GetBezierPoint();
                    else
                        pointY = v.GetBezierPoint();

                    var point = CalculatePatchPoint(matX, matY, matZ, pointX, pointY);
                    SceneManager.DrawPoint(bmp, g, point, Thickness, Color);
                }
            }
        }
        protected override Vector4[,] GetPatchMatrix(int i, int j)
        {
            const int bezierSegmentPoints = SceneManager.BezierSegmentPoints;
            return new[,]{{Points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints].TransformedPosition, Points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 1].TransformedPosition, Points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 2].TransformedPosition, Points[i * bezierSegmentPoints + 0, (j * bezierSegmentPoints + 3) % Points.GetLength(1)].TransformedPosition}
                        , {Points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints].TransformedPosition, Points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 1].TransformedPosition, Points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 2].TransformedPosition, Points[i * bezierSegmentPoints + 1, (j * bezierSegmentPoints + 3) % Points.GetLength(1)].TransformedPosition}
                        , {Points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints].TransformedPosition, Points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 1].TransformedPosition, Points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 2].TransformedPosition, Points[i * bezierSegmentPoints + 2, (j * bezierSegmentPoints + 3) % Points.GetLength(1)].TransformedPosition}
                        , {Points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints].TransformedPosition, Points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 1].TransformedPosition, Points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 2].TransformedPosition, Points[i * bezierSegmentPoints + 3, (j * bezierSegmentPoints + 3) % Points.GetLength(1)].TransformedPosition}};
        }
        #endregion Protected Methods
        #region Public Methods
        public override void Draw()
        {
            base.Draw();
            var manager = PatchManager.Instance;
            double maxX, maxY, minX, minY;
            Points.FindMaxMinCoords(out minX, out minY, out maxX, out maxY);

            var xDiv = (maxX - minX) * 4;
            var yDiv = (maxY - minY) * 4;

            Bitmap bmp = SceneManager.Instance.SceneImage;
            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < VerticalPatches; i++)
                {
                    for (int j = 0; j < HorizontalPatches; j++)
                    {
                        var matrix = GetPatchMatrix(i, j);

                        var matX = new Matrix3D(matrix[0, 0].X, matrix[0, 1].X, matrix[0, 2].X, matrix[0, 3].X
                                              , matrix[1, 0].X, matrix[1, 1].X, matrix[1, 2].X, matrix[1, 3].X
                                              , matrix[2, 0].X, matrix[2, 1].X, matrix[2, 2].X, matrix[2, 3].X
                                              , matrix[3, 0].X, matrix[3, 1].X, matrix[3, 2].X, matrix[3, 3].X);
                        var matY = new Matrix3D(matrix[0, 0].Y, matrix[0, 1].Y, matrix[0, 2].Y, matrix[0, 3].Y
                                              , matrix[1, 0].Y, matrix[1, 1].Y, matrix[1, 2].Y, matrix[1, 3].Y
                                              , matrix[2, 0].Y, matrix[2, 1].Y, matrix[2, 2].Y, matrix[2, 3].Y
                                              , matrix[3, 0].Y, matrix[3, 1].Y, matrix[3, 2].Y, matrix[3, 3].Y);
                        var matZ = new Matrix3D(matrix[0, 0].Z, matrix[0, 1].Z, matrix[0, 2].Z, matrix[0, 3].Z
                                              , matrix[1, 0].Z, matrix[1, 1].Z, matrix[1, 2].Z, matrix[1, 3].Z
                                              , matrix[2, 0].Z, matrix[2, 1].Z, matrix[2, 2].Z, matrix[2, 3].Z
                                              , matrix[3, 0].Z, matrix[3, 1].Z, matrix[3, 2].Z, matrix[3, 3].Z);

                        DrawSinglePatch(bmp, g, i, manager.VerticalPatchDivisions, matX, matY, matZ, (int)Math.Max(xDiv, yDiv), isHorizontal: false);
                        DrawSinglePatch(bmp, g, j, manager.HorizontalPatchDivisions, matX, matY, matZ, (int)Math.Max(xDiv, yDiv), isHorizontal: true);
                    }
                }
            }
            SceneManager.Instance.SceneImage = bmp;
        }
        public override Vector4 CalculatePatchPoint(int i, int j, double u, double v)
        {
            var pointX = u.GetBezierPoint();
            var pointY = v.GetBezierPoint();

            var matrix = GetPatchMatrix(i, j);

            var matX = new Matrix3D(matrix[0, 0].X, matrix[0, 1].X, matrix[0, 2].X, matrix[0, 3].X
                                  , matrix[1, 0].X, matrix[1, 1].X, matrix[1, 2].X, matrix[1, 3].X
                                  , matrix[2, 0].X, matrix[2, 1].X, matrix[2, 2].X, matrix[2, 3].X
                                  , matrix[3, 0].X, matrix[3, 1].X, matrix[3, 2].X, matrix[3, 3].X);
            var matY = new Matrix3D(matrix[0, 0].Y, matrix[0, 1].Y, matrix[0, 2].Y, matrix[0, 3].Y
                                  , matrix[1, 0].Y, matrix[1, 1].Y, matrix[1, 2].Y, matrix[1, 3].Y
                                  , matrix[2, 0].Y, matrix[2, 1].Y, matrix[2, 2].Y, matrix[2, 3].Y
                                  , matrix[3, 0].Y, matrix[3, 1].Y, matrix[3, 2].Y, matrix[3, 3].Y);
            var matZ = new Matrix3D(matrix[0, 0].Z, matrix[0, 1].Z, matrix[0, 2].Z, matrix[0, 3].Z
                                  , matrix[1, 0].Z, matrix[1, 1].Z, matrix[1, 2].Z, matrix[1, 3].Z
                                  , matrix[2, 0].Z, matrix[2, 1].Z, matrix[2, 2].Z, matrix[2, 3].Z
                                  , matrix[3, 0].Z, matrix[3, 1].Z, matrix[3, 2].Z, matrix[3, 3].Z);

            return CalculatePatchPoint(matX, matY, matZ, pointX, pointY);
        }
        #endregion Public Methods
    }
}

