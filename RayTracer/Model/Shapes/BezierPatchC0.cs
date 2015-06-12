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
        private void DrawNormalVector(Graphics graphics, double u, double v, Vector4 pointX, Vector4 pointY, Vector4 point, Matrix3D matX, Matrix3D matY, Matrix3D matZ)
        {
            var du = new Vector4(3 * u * u, 2 * u, 1, 0);
            var dv = new Vector4(3 * v * v, 2 * v, 1, 0);

            var nu = new Vector4(du * matX * pointY, du * matY * pointY, du * matZ * pointY, 1);
            var nv = new Vector4(pointX * matX * dv, pointX * matY * dv, pointX * matZ * dv, 1);

            var normal = nu.Cross(nv).Normalized;

            Vector4 endPoint = SceneManager.Instance.TransformMatrix * SceneManager.Instance.ScaleMatrix * Transformations.ViewMatrix(400) * new Vector4(normal.X + point.X, normal.Y + point.Y, normal.Z + point.Z, 1);
            Vector4 startPoint = SceneManager.Instance.TransformMatrix * SceneManager.Instance.ScaleMatrix * Transformations.ViewMatrix(400) * point;

            graphics.DrawLine(new Pen(Color.White) { Width = 1 }, (int)startPoint.X, (int)startPoint.Y, (int)endPoint.X, (int)endPoint.Y);
        }
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
                    pointY = new Vector4(Math.Pow((1.0 - u), 3), 3 * u * Math.Pow((1.0 - u), 2), 3 * u * u * (1.0 - u), Math.Pow(u, 3));
                else
                    pointX = new Vector4(Math.Pow((1.0 - u), 3), 3 * u * Math.Pow((1.0 - u), 2), 3 * u * u * (1.0 - u), Math.Pow(u, 3));

                for (int n = 0; n < divisions; n++)
                {
                    var v = n * drawingStep;
                    if (isHorizontal)
                        pointX = new Vector4(Math.Pow((1.0 - v), 3), 3 * v * Math.Pow((1.0 - v), 2), 3 * v * v * (1.0 - v), Math.Pow(v, 3));
                    else
                        pointY = new Vector4(Math.Pow((1.0 - v), 3), 3 * v * Math.Pow((1.0 - v), 2), 3 * v * v * (1.0 - v), Math.Pow(v, 3));

                    var x = pointX * matX * pointY;
                    var y = pointX * matY * pointY;
                    var z = pointX * matZ * pointY;
                    var point = new Vector4(x, y, z, 1);
                    SceneManager.DrawCurvePoint(bmp, g, point, Thickness);
                    // if (n == 0) DrawNormalVector(g, u, v, pointX, pointY, point, matX, matY, matZ);
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

            var xDiv = (maxX - minX) * 4;
            var yDiv = (maxY - minY) * 4;

            Bitmap bmp = SceneManager.Instance.SceneImage;
            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < VerticalPatches; i++)
                {
                    for (int j = 0; j < HorizontalPatches; j++)
                    {
                        Matrix3D matX = new Matrix3D(Points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints].TransformedPosition.X, Points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 1].TransformedPosition.X, Points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 2].TransformedPosition.X, Points[i * bezierSegmentPoints + 0, (j * bezierSegmentPoints + 3) % Points.GetLength(1)].TransformedPosition.X
                                                   , Points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints].TransformedPosition.X, Points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 1].TransformedPosition.X, Points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 2].TransformedPosition.X, Points[i * bezierSegmentPoints + 1, (j * bezierSegmentPoints + 3) % Points.GetLength(1)].TransformedPosition.X
                                                   , Points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints].TransformedPosition.X, Points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 1].TransformedPosition.X, Points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 2].TransformedPosition.X, Points[i * bezierSegmentPoints + 2, (j * bezierSegmentPoints + 3) % Points.GetLength(1)].TransformedPosition.X
                                                   , Points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints].TransformedPosition.X, Points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 1].TransformedPosition.X, Points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 2].TransformedPosition.X, Points[i * bezierSegmentPoints + 3, (j * bezierSegmentPoints + 3) % Points.GetLength(1)].TransformedPosition.X);
                        Matrix3D matY = new Matrix3D(Points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints].TransformedPosition.Y, Points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 1].TransformedPosition.Y, Points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 2].TransformedPosition.Y, Points[i * bezierSegmentPoints + 0, (j * bezierSegmentPoints + 3) % Points.GetLength(1)].TransformedPosition.Y
                                                   , Points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints].TransformedPosition.Y, Points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 1].TransformedPosition.Y, Points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 2].TransformedPosition.Y, Points[i * bezierSegmentPoints + 1, (j * bezierSegmentPoints + 3) % Points.GetLength(1)].TransformedPosition.Y
                                                   , Points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints].TransformedPosition.Y, Points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 1].TransformedPosition.Y, Points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 2].TransformedPosition.Y, Points[i * bezierSegmentPoints + 2, (j * bezierSegmentPoints + 3) % Points.GetLength(1)].TransformedPosition.Y
                                                   , Points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints].TransformedPosition.Y, Points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 1].TransformedPosition.Y, Points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 2].TransformedPosition.Y, Points[i * bezierSegmentPoints + 3, (j * bezierSegmentPoints + 3) % Points.GetLength(1)].TransformedPosition.Y);
                        Matrix3D matZ = new Matrix3D(Points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints].TransformedPosition.Z, Points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 1].TransformedPosition.Z, Points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 2].TransformedPosition.Z, Points[i * bezierSegmentPoints + 0, (j * bezierSegmentPoints + 3) % Points.GetLength(1)].TransformedPosition.Z
                                                   , Points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints].TransformedPosition.Z, Points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 1].TransformedPosition.Z, Points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 2].TransformedPosition.Z, Points[i * bezierSegmentPoints + 1, (j * bezierSegmentPoints + 3) % Points.GetLength(1)].TransformedPosition.Z
                                                   , Points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints].TransformedPosition.Z, Points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 1].TransformedPosition.Z, Points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 2].TransformedPosition.Z, Points[i * bezierSegmentPoints + 2, (j * bezierSegmentPoints + 3) % Points.GetLength(1)].TransformedPosition.Z
                                                   , Points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints].TransformedPosition.Z, Points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 1].TransformedPosition.Z, Points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 2].TransformedPosition.Z, Points[i * bezierSegmentPoints + 3, (j * bezierSegmentPoints + 3) % Points.GetLength(1)].TransformedPosition.Z);

                        DrawSinglePatch(bmp, g, i, manager.VerticalPatchDivisions, matX, matY, matZ, (int)Math.Max(xDiv, yDiv), isHorizontal: false);
                        DrawSinglePatch(bmp, g, j, manager.HorizontalPatchDivisions, matX, matY, matZ, (int)Math.Max(xDiv, yDiv), isHorizontal: true);
                    }
                }
            }
            SceneManager.Instance.SceneImage = bmp;
        }
        #endregion Public Methods
    }
}

