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
    public class GregoryPatch : ModelBase
    {
        #region Private Members
        private readonly Matrix3D BezierPolygonMatrix = new Matrix3D(1, -5 / 6.0, 2 / 6.0, 0
                                                                   , 0, 3, -9 / 6.0, 0
                                                                   , 0, -9 / 6.0, 3, 0
                                                                   , 0, 2 / 6.0, -5 / 6.0, 1);

        private IEnumerable<Vector4> _bezierPatchInnerPolygon = new List<Vector4>();
        private List<BezierPatch> _patches;
        /// <summary>
        /// p0          p1          p2        p3
        /// p4        p5 p6       p7 p8       p9
        /// p10      p11 p12     p13 p14      p15
        /// p16         p17         p18       p19     
        /// </summary>
        private PointEx[][] _points;
        #endregion Private Members
        #region Public Properties
        public override string Type { get { return "GregoryPatch"; } }
        public override IEnumerable<ShapeBase> SelectedItems { get { return Vertices.Where(v => v.IsSelected); } }
        #endregion Public Properties
        #region Constructors
        public GregoryPatch(double x, double y, double z, string name, List<BezierPatch> patches)
            : base(x, y, z, name)
        {
            _patches = SortInputPatches(patches);
            DisplayEdges = true;
        }
        #endregion Constructors
        #region Private Methods
        private void FindGregoryPatches()
        {
            _bezierPatchInnerPolygon = new List<Vector4>();
            _points = new PointEx[3][];
            for (int i = 0; i < _points.Length; i++)
                _points[i] = new PointEx[20];

            for (int i = 0; i < _patches.Count; i++)
            {
                var patch = _patches[i];
                var points = patch.Points;
                Matrix3D matX = new Matrix3D(points[0, 0].TransformedPosition.X, points[0, 1].TransformedPosition.X, points[0, 2].TransformedPosition.X, points[0, 3].TransformedPosition.X
                                           , points[1, 0].TransformedPosition.X, points[1, 1].TransformedPosition.X, points[1, 2].TransformedPosition.X, points[1, 3].TransformedPosition.X
                                           , points[2, 0].TransformedPosition.X, points[2, 1].TransformedPosition.X, points[2, 2].TransformedPosition.X, points[2, 3].TransformedPosition.X
                                           , points[3, 0].TransformedPosition.X, points[3, 1].TransformedPosition.X, points[3, 2].TransformedPosition.X, points[3, 3].TransformedPosition.X);
                Matrix3D matY = new Matrix3D(points[0, 0].TransformedPosition.Y, points[0, 1].TransformedPosition.Y, points[0, 2].TransformedPosition.Y, points[0, 3].TransformedPosition.Y
                                           , points[1, 0].TransformedPosition.Y, points[1, 1].TransformedPosition.Y, points[1, 2].TransformedPosition.Y, points[1, 3].TransformedPosition.Y
                                           , points[2, 0].TransformedPosition.Y, points[2, 1].TransformedPosition.Y, points[2, 2].TransformedPosition.Y, points[2, 3].TransformedPosition.Y
                                           , points[3, 0].TransformedPosition.Y, points[3, 1].TransformedPosition.Y, points[3, 2].TransformedPosition.Y, points[3, 3].TransformedPosition.Y);
                Matrix3D matZ = new Matrix3D(points[0, 0].TransformedPosition.Z, points[0, 1].TransformedPosition.Z, points[0, 2].TransformedPosition.Z, points[0, 3].TransformedPosition.Z
                                           , points[1, 0].TransformedPosition.Z, points[1, 1].TransformedPosition.Z, points[1, 2].TransformedPosition.Z, points[1, 3].TransformedPosition.Z
                                           , points[2, 0].TransformedPosition.Z, points[2, 1].TransformedPosition.Z, points[2, 2].TransformedPosition.Z, points[2, 3].TransformedPosition.Z
                                           , points[3, 0].TransformedPosition.Z, points[3, 1].TransformedPosition.Z, points[3, 2].TransformedPosition.Z, points[3, 3].TransformedPosition.Z);

                CalculateGregoryPoints(patch, matX, matY, matZ, i);
            }
            CalculateMiddlePoint();
            CalculateMissingInnerPoints();
        }
        private void CalculateMissingInnerPoints()
        {
            for (int i = 0; i < _points.GetLength(0); i++)
            {
                var p18 = _points[i][17].Vector4 + ((_points[i][19].Vector4 - _points[i][17].Vector4) * 0.5);
                var p18Ex = new PointEx(p18.X, p18.Y, p18.Z);
                _points[i][18] = p18Ex;
                _points[(i + 1) % 3][15] = p18Ex;

                var p13 = _points[i][18].Vector4 + ((_points[i][7].Vector4 - _points[i][18].Vector4) / 3);
                _points[i][13] = new PointEx(p13.X, p13.Y, p13.Z);

                p13 = _points[i][18].Vector4 - ((_points[i][7].Vector4 - _points[i][18].Vector4) / 2);
                _points[(i + 1) % 3][14] = new PointEx(p13.X, p13.Y, p13.Z);

                var p12 = _points[i][17].Vector4 + ((_points[i][5].Vector4 - _points[i][17].Vector4) / 2);
                _points[i][12] = new PointEx(p12.X, p12.Y, p12.Z);
                p12 = _points[i][17].Vector4 + ((_points[i][17].Vector4 - _points[i][5].Vector4) / 2);
                _points[(i + 1) % 3][8] = new PointEx(p12.X, p12.Y, p12.Z);
            }
        }
        private void CalculateMiddlePoint()
        {
            var x1 = _points[0][16].Vector4 + (_points[0][17].Vector4 - _points[0][16].Vector4) * (3 / 2.0);
            var x2 = _points[1][16].Vector4 + (_points[1][17].Vector4 - _points[1][16].Vector4) * (3 / 2.0);
            var x3 = _points[2][16].Vector4 + (_points[2][17].Vector4 - _points[2][16].Vector4) * (3 / 2.0);

            var midPoint = (x1 + x2 + x3) / 3;
            var mid = new PointEx(midPoint.X, midPoint.Y, midPoint.Z);
            _points[0][19] = _points[1][19] = _points[2][19] = mid;
        }
        private List<BezierPatch> SortInputPatches(List<BezierPatch> inputPatches)
        {
            var patches = new List<BezierPatch>();
            var firstPatch = inputPatches.ElementAt(0);
            patches.Add(firstPatch);
            inputPatches.Remove(firstPatch);

            var nextPatch = inputPatches.First(p => p.Vertices.Contains(firstPatch.CommonPoints[1]));
            patches.Add(nextPatch);
            inputPatches.Remove(nextPatch);
            patches.Add(inputPatches.First());

            return patches;
        }
        private void CalculateGregoryPoints(BezierPatch patch, Matrix3D matX, Matrix3D matY, Matrix3D matZ, int patchIndex)
        {
            // start <x, y>
            var start = patch.Points.CoordinatesOf(patch.CommonPoints[0]);
            var end = patch.Points.CoordinatesOf(patch.CommonPoints[1]);
            bool shouldBeReversed = (start.Item1 == 3 && end.Item1 == 3) || (start.Item2 == 0 && end.Item2 == 0);

            // If the edge is horizontal
            if (start.Item1 == end.Item1)
                CalculatePointsForHorizontalEdge(matX, matY, matZ, patchIndex, shouldBeReversed, isStartPoint: start.Item1 == 0);
            else // if the edge is vertical
                CalculatePointsForVerticalEdge(matX, matY, matZ, patchIndex, shouldBeReversed, isStartPoint: start.Item2 == 0);
        }
        private void CalculatePointsForVerticalEdge(Matrix3D matX, Matrix3D matY, Matrix3D matZ, int patchIndex, bool shouldBeReversed, bool isStartPoint)
        {
            var edge = new Vector4[7];
            var innerEdge = new Vector4[7];
            Vector4 v = isStartPoint ? new Vector4(1, 0, 0, 0) : new Vector4(0, 0, 0, 1);
            double innerDivision = isStartPoint ? 1 / 6.0 : 5 / 6.0;
            Vector4 vInner = new Vector4(Math.Pow((1.0 - innerDivision), 3), 3 * innerDivision * Math.Pow((1.0 - innerDivision), 2), 3 * innerDivision * innerDivision * (1.0 - innerDivision), Math.Pow(innerDivision, 3));

            for (int i = 0; i < 7; i++)
            {
                var p = i / 6.0;
                Vector4 u = new Vector4(Math.Pow((1.0 - p), 3), 3 * p * Math.Pow((1.0 - p), 2), 3 * p * p * (1.0 - p), Math.Pow(p, 3));

                var x = u * matX * v;
                var y = u * matY * v;
                var z = u * matZ * v;

                var xi = u * matX * vInner;
                var yi = u * matY * vInner;
                var zi = u * matZ * vInner;

                edge[i] = new Vector4(x, y, z, 1);
                innerEdge[i] = new Vector4(xi, yi, zi, 1);
            }

            InterpolateBezierPolygon(edge, innerEdge, patchIndex, shouldBeReversed);
        }
        private void CalculatePointsForHorizontalEdge(Matrix3D matX, Matrix3D matY, Matrix3D matZ, int patchIndex, bool shouldBeReversed, bool isStartPoint)
        {
            var edge = new Vector4[7];
            var innerEdge = new Vector4[7];
            Vector4 u = isStartPoint ? new Vector4(1, 0, 0, 0) : new Vector4(0, 0, 0, 1);
            double innerDivision = isStartPoint ? 1 / 6.0 : 5 / 6.0;
            Vector4 uInner = new Vector4(Math.Pow((1.0 - innerDivision), 3), 3 * innerDivision * Math.Pow((1.0 - innerDivision), 2), 3 * innerDivision * innerDivision * (1.0 - innerDivision), Math.Pow(innerDivision, 3));

            for (int i = 0; i < 7; i++)
            {
                var p = i / 6.0;
                Vector4 v = new Vector4(Math.Pow((1.0 - p), 3), 3 * p * Math.Pow((1.0 - p), 2), 3 * p * p * (1.0 - p), Math.Pow(p, 3));

                var x = u * matX * v;
                var y = u * matY * v;
                var z = u * matZ * v;

                var xi = uInner * matX * v;
                var yi = uInner * matY * v;
                var zi = uInner * matZ * v;

                edge[i] = new Vector4(x, y, z, 1);
                innerEdge[i] = new Vector4(xi, yi, zi, 1);
            }

            InterpolateBezierPolygon(edge, innerEdge, patchIndex, shouldBeReversed);
        }
        private void InterpolateBezierPolygon(Vector4[] edge, Vector4[] innerEdge, int patchIndex, bool shouldBeReversed)
        {
            var polygon = FindEdgePolygon(edge, patchIndex, shouldBeReversed);
            var innerPolygon = FindInnerPolygon(innerEdge, shouldBeReversed);

            var point = polygon[1] + polygon[1] - innerPolygon[1];
            _points[patchIndex][5] = new PointEx(point.X, point.Y, point.Z);

            point = polygon[2] + polygon[2] - innerPolygon[2];
            _points[patchIndex][11] = new PointEx(point.X, point.Y, point.Z);

            point = polygon[3] + polygon[3] - innerPolygon[3];
            var p = new PointEx(point.X, point.Y, point.Z);
            _points[patchIndex][17] = p;
            _points[(patchIndex + 1) % 3][9] = p;

            point = polygon[5] + polygon[5] - innerPolygon[5];
            _points[(patchIndex + 1) % 3][7] = new PointEx(point.X, point.Y, point.Z);

            point = polygon[6] + polygon[6] - innerPolygon[6];
            _points[(patchIndex + 1) % 3][6] = new PointEx(point.X, point.Y, point.Z);
        }
        private List<Vector4> FindEdgePolygon(Vector4[] edge, int patchIndex, bool shouldBeReversed)
        {
            var points = new List<Vector4>();
            Vector4 p1, p2, p3, p4;

            var p = new Matrix3D(edge[0].X, edge[1].X, edge[2].X, edge[3].X
                , edge[0].Y, edge[1].Y, edge[2].Y, edge[3].Y
                , edge[0].Z, edge[1].Z, edge[2].Z, edge[3].Z
                , 1, 1, 1, 1) * BezierPolygonMatrix;

            points.Add(p1 = new Vector4(p.M11, p.M21, p.M31, p.OffsetX).Normalized);
            points.Add(p2 = new Vector4(p.M12, p.M22, p.M32, p.OffsetY).Normalized);
            points.Add(p3 = new Vector4(p.M13, p.M23, p.M33, p.OffsetZ).Normalized);
            points.Add(p4 = new Vector4(p.M14, p.M24, p.M34, p.M44).Normalized);

            if (!shouldBeReversed)
            {
                _points[patchIndex][0] = new PointEx(p1.X, p1.Y, p1.Z);
                _points[patchIndex][4] = new PointEx(p2.X, p2.Y, p2.Z);
                _points[patchIndex][10] = new PointEx(p3.X, p3.Y, p3.Z);
                _points[patchIndex][16] = new PointEx(p4.X, p4.Y, p4.Z);
            }
            else
            {
                _points[(patchIndex + 1) % 3][0] = new PointEx(p1.X, p1.Y, p1.Z);
                _points[(patchIndex + 1) % 3][1] = new PointEx(p2.X, p2.Y, p2.Z);
                _points[(patchIndex + 1) % 3][2] = new PointEx(p3.X, p3.Y, p3.Z);
                _points[(patchIndex + 1) % 3][3] = new PointEx(p4.X, p4.Y, p4.Z);
            }

            p = new Matrix3D(edge[3].X, edge[4].X, edge[5].X, edge[6].X
                , edge[3].Y, edge[4].Y, edge[5].Y, edge[6].Y
                , edge[3].Z, edge[4].Z, edge[5].Z, edge[6].Z
                , 1, 1, 1, 1) * BezierPolygonMatrix;

            points.Add(p1 = new Vector4(p.M11, p.M21, p.M31, p.OffsetX).Normalized);
            points.Add(p2 = new Vector4(p.M12, p.M22, p.M32, p.OffsetY).Normalized);
            points.Add(p3 = new Vector4(p.M13, p.M23, p.M33, p.OffsetZ).Normalized);
            points.Add(p4 = new Vector4(p.M14, p.M24, p.M34, p.M44).Normalized);

            if (!shouldBeReversed)
            {
                _points[(patchIndex + 1) % 3][3] = new PointEx(p1.X, p1.Y, p1.Z);
                _points[(patchIndex + 1) % 3][2] = new PointEx(p2.X, p2.Y, p2.Z);
                _points[(patchIndex + 1) % 3][1] = new PointEx(p3.X, p3.Y, p3.Z);
                _points[(patchIndex + 1) % 3][0] = new PointEx(p4.X, p4.Y, p4.Z);
            }
            else
            {
                _points[patchIndex][16] = new PointEx(p1.X, p1.Y, p1.Z);
                _points[patchIndex][10] = new PointEx(p2.X, p2.Y, p2.Z);
                _points[patchIndex][4] = new PointEx(p3.X, p3.Y, p3.Z);
                _points[patchIndex][0] = new PointEx(p4.X, p4.Y, p4.Z);
                points.Reverse();
            }
            return points;
        }
        private List<Vector4> FindInnerPolygon(Vector4[] innerEdge, bool shouldBeReversed)
        {
            var points = new List<Vector4>();

            var p = new Matrix3D(innerEdge[0].X, innerEdge[1].X, innerEdge[2].X, innerEdge[3].X
                , innerEdge[0].Y, innerEdge[1].Y, innerEdge[2].Y, innerEdge[3].Y
                , innerEdge[0].Z, innerEdge[1].Z, innerEdge[2].Z, innerEdge[3].Z
                , 1, 1, 1, 1) * BezierPolygonMatrix;

            points.Add(new Vector4(p.M11, p.M21, p.M31, p.OffsetX).Normalized);
            points.Add(new Vector4(p.M12, p.M22, p.M32, p.OffsetY).Normalized);
            points.Add(new Vector4(p.M13, p.M23, p.M33, p.OffsetZ).Normalized);
            points.Add(new Vector4(p.M14, p.M24, p.M34, p.M44).Normalized);

            p = new Matrix3D(innerEdge[3].X, innerEdge[4].X, innerEdge[5].X, innerEdge[6].X
                , innerEdge[3].Y, innerEdge[4].Y, innerEdge[5].Y, innerEdge[6].Y
                , innerEdge[3].Z, innerEdge[4].Z, innerEdge[5].Z, innerEdge[6].Z
                , 1, 1, 1, 1) * BezierPolygonMatrix;

            points.Add(new Vector4(p.M11, p.M21, p.M31, p.OffsetX).Normalized);
            points.Add(new Vector4(p.M12, p.M22, p.M32, p.OffsetY).Normalized);
            points.Add(new Vector4(p.M13, p.M23, p.M33, p.OffsetZ).Normalized);
            points.Add(new Vector4(p.M14, p.M24, p.M34, p.M44).Normalized);

            if (shouldBeReversed)
                points.Reverse();

            _bezierPatchInnerPolygon = _bezierPatchInnerPolygon.Union(points);

            return points;
        }
        #endregion Private Methods
        #region Public Methods
        public override void Draw()
        {
            FindGregoryPatches();
            Bitmap bmp = SceneManager.Instance.SceneImage;

            using (Graphics g = Graphics.FromImage(bmp))
            {
                if (DisplayEdges)
                    DrawHelperLines(bmp, g);
                for (int i = 0; i < _points.GetLength(0); i++)
                    DrawGregoryPatch(bmp, g, i);
            }
            SceneManager.Instance.SceneImage = bmp;
        }
        private void DrawGregoryPatch(Bitmap bmp, Graphics g, int patchIndex)
        {
            Vector4[,] points, innerPoints;
            CalculateControlPoints(patchIndex, out points, out innerPoints);
            DrawSingleGregoryPatch(bmp, g, patchIndex, PatchManager.Instance.HorizontalPatchDivisions, 100, points, innerPoints, isHorizontal: true);
            DrawSingleGregoryPatch(bmp, g, patchIndex, PatchManager.Instance.HorizontalPatchDivisions, 100, points, innerPoints, isHorizontal: false);
        }
        private void DrawSingleGregoryPatch(Bitmap bmp, Graphics g, int patchIndex, int patchDivisions, int divisions, Vector4[,] points, Vector4[,] innerPoints, bool isHorizontal)
        {
            Matrix3D matX, matY, matZ;
            double step = 1.0f / (patchDivisions - 1);
            double drawingStep = 1.0f / (divisions - 1);
            double currentStep = 0;
            Vector4 pointX = null, pointY = null;

            for (double m = 0; m < patchDivisions; m++, currentStep += step)
            {
                if (isHorizontal)
                    pointY = new Vector4(Math.Pow((1.0 - currentStep), 3), 3 * currentStep * Math.Pow((1.0 - currentStep), 2), 3 * currentStep * currentStep * (1.0 - currentStep), Math.Pow(currentStep, 3));
                else
                    pointX = new Vector4(Math.Pow((1.0 - currentStep), 3), 3 * currentStep * Math.Pow((1.0 - currentStep), 2), 3 * currentStep * currentStep * (1.0 - currentStep), Math.Pow(currentStep, 3));

                for (double n = 0; n < divisions; n++)
                {
                    var point = n * drawingStep;
                    if (isHorizontal)
                        pointX = new Vector4(Math.Pow((1.0 - point), 3), 3 * point * Math.Pow((1.0 - point), 2), 3 * point * point * (1.0 - point), Math.Pow(point, 3));
                    else
                        pointY = new Vector4(Math.Pow((1.0 - point), 3), 3 * point * Math.Pow((1.0 - point), 2), 3 * point * point * (1.0 - point), Math.Pow(point, 3));

                    SetMatrix(currentStep, point, out matX, out matY, out matZ, points, innerPoints);

                    var x = pointX * matX * pointY;
                    var y = pointX * matY * pointY;
                    var z = pointX * matZ * pointY;

                    var p = new Vector4(x, y, z, 1);
                    SceneManager.DrawCurvePoint(bmp, g, p, Thickness);
                }
            }
        }
        private void CalculateControlPoints(int patchIndex, out Vector4[,] points, out Vector4[,] innerPoints)
        {
            points = new Vector4[4, 4];
            innerPoints = new Vector4[2, 2];

            points[0, 0] = _points[patchIndex][16].Vector4;
            points[0, 1] = _points[patchIndex][10].Vector4;
            points[0, 2] = _points[patchIndex][4].Vector4;
            points[0, 3] = _points[patchIndex][0].Vector4;
            points[1, 0] = _points[patchIndex][17].Vector4;
            points[1, 1] = _points[patchIndex][12].Vector4;
            points[1, 2] = _points[patchIndex][6].Vector4;
            points[1, 3] = _points[patchIndex][1].Vector4;
            points[2, 0] = _points[patchIndex][18].Vector4;
            points[2, 1] = _points[patchIndex][13].Vector4;
            points[2, 2] = _points[patchIndex][7].Vector4;
            points[2, 3] = _points[patchIndex][2].Vector4;
            points[3, 0] = _points[patchIndex][19].Vector4;
            points[3, 1] = _points[patchIndex][15].Vector4;
            points[3, 2] = _points[patchIndex][9].Vector4;
            points[3, 3] = _points[patchIndex][3].Vector4;

            innerPoints[0, 0] = _points[patchIndex][11].Vector4;
            innerPoints[0, 1] = _points[patchIndex][5].Vector4;
            innerPoints[1, 0] = _points[patchIndex][14].Vector4;
            innerPoints[1, 1] = _points[patchIndex][8].Vector4;
        }
        private void DrawHelperLines(Bitmap bmp, Graphics g)
        {
            Transform = Transformations.ViewMatrix(400);
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 20; i++)
                {
                    if (_points[j][i] == null) continue;
                    var PointOnScreen = Transform * _points[j][i].PointOnScreen;
                    if (double.IsNaN(PointOnScreen.X) || double.IsNaN(PointOnScreen.Y) || PointOnScreen.X < 0 ||
                        PointOnScreen.X >= bmp.Width || PointOnScreen.Y < 0 || PointOnScreen.Y >= bmp.Height) continue;
                    Color color = bmp.GetPixel((int)PointOnScreen.X, (int)PointOnScreen.Y);
                    if (i == 6 || i == 14)
                        g.FillRectangle(new SolidBrush(color.CombinedColor(Color.DeepPink)), (int)PointOnScreen.X,
                            (int)PointOnScreen.Y, 5, 5);
                    else if (i == 13 || i == 14)
                        g.FillRectangle(new SolidBrush(color.CombinedColor(Color.Yellow)), (int)PointOnScreen.X,
                            (int)PointOnScreen.Y, 6, 6);
                    else if (i == 7)
                        g.FillRectangle(new SolidBrush(color.CombinedColor(Color.Blue)), (int)PointOnScreen.X,
                            (int)PointOnScreen.Y, 5, 5);
                    else if (i == 6 || i == 7 || i == 8)
                        g.FillRectangle(new SolidBrush(color.CombinedColor(Color.Blue)), (int)PointOnScreen.X,
                            (int)PointOnScreen.Y, 3, 3);
                    else if (i == 19)
                        g.FillRectangle(new SolidBrush(color.CombinedColor(Color.White)), (int)PointOnScreen.X,
                            (int)PointOnScreen.Y, 5, 5);
                    else
                        g.FillRectangle(new SolidBrush(color.CombinedColor(Color.Lime)), (int)PointOnScreen.X,
                            (int)PointOnScreen.Y, 3, 3);
                }
                DrawGregoryPatchBorders(g, j);
                DrawGregoryPatchHelperEdges(g, j);
                DrawInnerPatchCurvePolygon(bmp, g);
            }
        }
        private void DrawInnerPatchCurvePolygon(Bitmap bmp, Graphics g)
        {
            for (int i = 0; i < _bezierPatchInnerPolygon.Count(); i++)
            {
                var point = _bezierPatchInnerPolygon.ElementAt(i);
                var PointOnScreen = Transform * point;
                if (double.IsNaN(PointOnScreen.X) || double.IsNaN(PointOnScreen.Y) || PointOnScreen.X < 0 ||
                    PointOnScreen.X >= bmp.Width || PointOnScreen.Y < 0 || PointOnScreen.Y >= bmp.Height) continue;
                Color color = bmp.GetPixel((int)PointOnScreen.X, (int)PointOnScreen.Y);
                g.FillRectangle(new SolidBrush(color.CombinedColor(Color.OrangeRed)), (int)PointOnScreen.X, (int)PointOnScreen.Y, 3, 3);
                //g.DrawLine(new Pen(Color.MediumSeaGreen), (float)pt.X, (float)pt.Y, (float)pt1.X, (float)pt1.Y);
            }
        }
        private void DrawGregoryPatchHelperEdges(Graphics g, int j)
        {
            var pt = Transform * _points[j][4].PointOnScreen;
            var pt1 = Transform * _points[j][5].PointOnScreen;
            g.DrawLine(new Pen(Color.MediumSeaGreen), (float)pt.X, (float)pt.Y, (float)pt1.X, (float)pt1.Y);
            pt = Transform * _points[j][10].PointOnScreen;
            pt1 = Transform * _points[j][11].PointOnScreen;
            g.DrawLine(new Pen(Color.MediumSeaGreen), (float)pt.X, (float)pt.Y, (float)pt1.X, (float)pt1.Y);
            pt = Transform * _points[j][1].PointOnScreen;
            pt1 = Transform * _points[j][6].PointOnScreen;
            g.DrawLine(new Pen(Color.MediumSeaGreen), (float)pt.X, (float)pt.Y, (float)pt1.X, (float)pt1.Y);
            pt = Transform * _points[j][2].PointOnScreen;
            pt1 = Transform * _points[j][7].PointOnScreen;
            g.DrawLine(new Pen(Color.MediumSeaGreen), (float)pt.X, (float)pt.Y, (float)pt1.X, (float)pt1.Y);
            pt = Transform * _points[j][17].PointOnScreen;
            pt1 = Transform * _points[j][12].PointOnScreen;
            g.DrawLine(new Pen(Color.Blue), (float)pt.X, (float)pt.Y, (float)pt1.X, (float)pt1.Y);
            pt = Transform * _points[j][18].PointOnScreen;
            pt1 = Transform * _points[j][13].PointOnScreen;
            g.DrawLine(new Pen(Color.Blue), (float)pt.X, (float)pt.Y, (float)pt1.X, (float)pt1.Y);
            pt = Transform * _points[j][8].PointOnScreen;
            pt1 = Transform * _points[j][9].PointOnScreen;
            g.DrawLine(new Pen(Color.Blue), (float)pt.X, (float)pt.Y, (float)pt1.X, (float)pt1.Y);
            pt = Transform * _points[j][14].PointOnScreen;
            pt1 = Transform * _points[j][15].PointOnScreen;
            g.DrawLine(new Pen(Color.Blue), (float)pt.X, (float)pt.Y, (float)pt1.X, (float)pt1.Y);
        }
        private void DrawGregoryPatchBorders(Graphics g, int j)
        {
            var pt = Transform * _points[j][16].PointOnScreen;
            var pt1 = Transform * _points[j][17].PointOnScreen;
            g.DrawLine(new Pen(Color.Crimson), (float)pt.X, (float)pt.Y, (float)pt1.X, (float)pt1.Y);
            pt = Transform * _points[j][17].PointOnScreen;
            pt1 = Transform * _points[j][18].PointOnScreen;
            g.DrawLine(new Pen(Color.Crimson), (float)pt.X, (float)pt.Y, (float)pt1.X, (float)pt1.Y);
            pt = Transform * _points[j][18].PointOnScreen;
            pt1 = Transform * _points[j][19].PointOnScreen;
            g.DrawLine(new Pen(Color.Crimson), (float)pt.X, (float)pt.Y, (float)pt1.X, (float)pt1.Y);
            pt = Transform * _points[j][3].PointOnScreen;
            pt1 = Transform * _points[j][9].PointOnScreen;
            g.DrawLine(new Pen(Color.Crimson), (float)pt.X, (float)pt.Y, (float)pt1.X, (float)pt1.Y);
            pt = Transform * _points[j][9].PointOnScreen;
            pt1 = Transform * _points[j][15].PointOnScreen;
            g.DrawLine(new Pen(Color.Crimson), (float)pt.X, (float)pt.Y, (float)pt1.X, (float)pt1.Y);
            pt = Transform * _points[j][15].PointOnScreen;
            pt1 = Transform * _points[j][19].PointOnScreen;
            g.DrawLine(new Pen(Color.Crimson), (float)pt.X, (float)pt.Y, (float)pt1.X, (float)pt1.Y);
        }
        public void SetMatrix(double u, double v, out Matrix3D matX, out Matrix3D matY, out Matrix3D matZ, Vector4[,] points, Vector4[,] innerPoints)
        {
            Vector4[,] BasePoints = new Vector4[4, 4];
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    BasePoints[i, j] = CalculatePFromQ(i, j, u, v, points, innerPoints);

            matX = new Matrix3D(BasePoints[0, 0].X, BasePoints[0, 1].X, BasePoints[0, 2].X, BasePoints[0, 3].X,
                                BasePoints[1, 0].X, BasePoints[1, 1].X, BasePoints[1, 2].X, BasePoints[1, 3].X,
                                BasePoints[2, 0].X, BasePoints[2, 1].X, BasePoints[2, 2].X, BasePoints[2, 3].X,
                                BasePoints[3, 0].X, BasePoints[3, 1].X, BasePoints[3, 2].X, BasePoints[3, 3].X);

            matY = new Matrix3D(BasePoints[0, 0].Y, BasePoints[0, 1].Y, BasePoints[0, 2].Y, BasePoints[0, 3].Y,
                                BasePoints[1, 0].Y, BasePoints[1, 1].Y, BasePoints[1, 2].Y, BasePoints[1, 3].Y,
                                BasePoints[2, 0].Y, BasePoints[2, 1].Y, BasePoints[2, 2].Y, BasePoints[2, 3].Y,
                                BasePoints[3, 0].Y, BasePoints[3, 1].Y, BasePoints[3, 2].Y, BasePoints[3, 3].Y);

            matZ = new Matrix3D(BasePoints[0, 0].Z, BasePoints[0, 1].Z, BasePoints[0, 2].Z, BasePoints[0, 3].Z,
                                BasePoints[1, 0].Z, BasePoints[1, 1].Z, BasePoints[1, 2].Z, BasePoints[1, 3].Z,
                                BasePoints[2, 0].Z, BasePoints[2, 1].Z, BasePoints[2, 2].Z, BasePoints[2, 3].Z,
                                BasePoints[3, 0].Z, BasePoints[3, 1].Z, BasePoints[3, 2].Z, BasePoints[3, 3].Z);
        }
        private Vector4 CalculatePFromQ(int i, int j, double u, double v, Vector4[,] points, Vector4[,] innerPoints)
        {
            Vector4 p;
            if (i == 1 && j == 1)
                p = (u == 0 && v == 0) ? points[i, j] : (points[i, j] * u + innerPoints[i - 1, j - 1] * v) / (u + v);
            else if (i == 1 && j == 2)
                p = (u == 0 && v == 1) ? points[i, j] : (points[i, j] * u + innerPoints[i - 1, j - 1] * (1 - v)) / (u + 1 - v);
            else if (i == 2 && j == 1)
                p = (u == 1 && v == 0) ? points[i, j] : (points[i, j] * (1 - u) + innerPoints[i - 1, j - 1] * v) / (1 - u + v);
            else if (i == 2 && j == 2)
                p = (u == 1 && v == 1) ? points[i, j] : (points[i, j] * (1 - u) + innerPoints[i - 1, j - 1] * (1 - v)) / (1 - u + 1 - v);
            else
                p = points[i, j];
            //  p.Normalize();
            return new Vector4(p.X, p.Y, p.Z, 1);   //tutaj moze znormalizowac
        }
        #endregion Public Methods
    }
}

