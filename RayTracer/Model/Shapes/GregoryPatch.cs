using System;
using System.Collections.Generic;
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

        private IEnumerable<Vector4> aaaa = new List<Vector4>();
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
            FindGregoryPatches(patches);
        }
        #endregion Constructors
        #region Private Methods
        private void FindGregoryPatches(List<BezierPatch> inputPatches)
        {
            _points = new PointEx[3][];
            for (int i = 0; i < _points.Length; i++)
                _points[i] = new PointEx[20];

            var patches = SortInputPatches(inputPatches);
            for (int i = 0; i < patches.Count; i++)
            {
                var patch = patches[i];
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
                var p2 = (_points[i][1].Vector4 + ((_points[i][3].Vector4 - _points[i][1].Vector4) / 2)).Normalized;
                _points[i][2] = new PointEx(p2.X, p2.Y, p2.Z);

                var p7 = (_points[i][2].Vector4 + ((_points[i][13].Vector4 - _points[i][2].Vector4) / 2)).Normalized;
                _points[i][7] = new PointEx(p7.X, p7.Y, p7.Z);
                p7 = (_points[i][2].Vector4 - ((_points[i][13].Vector4 - _points[i][2].Vector4) / 2)).Normalized;
                _points[(i + 1) % 3][8] = new PointEx(p7.X, p7.Y, p7.Z);

                var p6 = (_points[i][1].Vector4 + ((_points[i][12].Vector4 - _points[i][1].Vector4) / 2)).Normalized;
                _points[i][6] = new PointEx(p6.X, p6.Y, p6.Z);
                p6 = (_points[i][1].Vector4 - ((_points[i][12].Vector4 - _points[i][1].Vector4) / 2)).Normalized;
                _points[(i + 1) % 3][14] = new PointEx(p6.X, p6.Y, p6.Z);
            }
        }
        private void CalculateMiddlePoint()
        {
            var x1 = _points[0][0].Vector4 + (_points[0][1].Vector4 - _points[0][0].Vector4) * (3 / 2.0);
            var x2 = _points[1][0].Vector4 + (_points[1][1].Vector4 - _points[1][0].Vector4) * (3 / 2.0);
            var x3 = _points[2][0].Vector4 + (_points[2][1].Vector4 - _points[2][0].Vector4) * (3 / 2.0);

            var midPoint = (x1 + x2 + x3) / 3;
            var mid = new PointEx(midPoint.X, midPoint.Y, midPoint.Z);
            _points[0][3] = _points[1][3] = _points[2][3] = mid;
        }
        private List<BezierPatch> SortInputPatches(List<BezierPatch> inputPatches)
        {
            var patches = new List<BezierPatch>();
            var firstPatch = inputPatches.ElementAt(0);
            patches.Add(firstPatch);
            inputPatches.Remove(firstPatch);

            var nextPatch = inputPatches.First(p => p.Vertices.Contains(firstPatch.CommonPoints[0]));
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

            // If the edge is horizontal
            if (start.Item1 == end.Item1)
                CalculatePointsForHorizontalEdge(matX, matY, matZ, patchIndex, isStartPoint: start.Item1 == 0);
            else // if the edge is vertical
                CalculatePointsForVerticalEdge(matX, matY, matZ, patchIndex, isStartPoint: start.Item2 == 0);
        }
        private void CalculatePointsForVerticalEdge(Matrix3D matX, Matrix3D matY, Matrix3D matZ, int patchIndex, bool isStartPoint)
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

            InterpolateBezierPolygon(edge, innerEdge, patchIndex);
        }
        private void CalculatePointsForHorizontalEdge(Matrix3D matX, Matrix3D matY, Matrix3D matZ, int patchIndex, bool isStartPoint)
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

            InterpolateBezierPolygon(edge, innerEdge, patchIndex);
        }
        private void InterpolateBezierPolygon(Vector4[] edge, Vector4[] innerEdge, int patchIndex)
        {
            var polygon = FindEdgePolygon(edge, patchIndex);
            var innerPolygon = FindInnerPolygon(innerEdge);

            var point = polygon[1] + polygon[1] - innerPolygon[1];
            _points[patchIndex][11] = new PointEx(point.X, point.Y, point.Z);

            point = polygon[2] + polygon[2] - innerPolygon[2];
            _points[patchIndex][5] = new PointEx(point.X, point.Y, point.Z);

            point = polygon[3] + polygon[3] - innerPolygon[3];
            var p = new PointEx(point.X, point.Y, point.Z);
            _points[patchIndex][1] = p;
            _points[(patchIndex + 1) % 3][15] = p;

            point = polygon[5] + polygon[5] - innerPolygon[5];
            _points[(patchIndex + 1) % 3][13] = new PointEx(point.X, point.Y, point.Z);

            point = polygon[6] + polygon[6] - innerPolygon[6];
            _points[(patchIndex + 1) % 3][12] = new PointEx(point.X, point.Y, point.Z);
        }
        private List<Vector4> FindEdgePolygon(Vector4[] edge, int patchIndex)
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

            _points[patchIndex][16] = new PointEx(p1.X, p1.Y, p1.Z);
            _points[patchIndex][10] = new PointEx(p2.X, p2.Y, p2.Z);
            _points[patchIndex][4] = new PointEx(p3.X, p3.Y, p3.Z);
            _points[patchIndex][0] = new PointEx(p4.X, p4.Y, p4.Z);

            p = new Matrix3D(edge[3].X, edge[4].X, edge[5].X, edge[6].X
                , edge[3].Y, edge[4].Y, edge[5].Y, edge[6].Y
                , edge[3].Z, edge[4].Z, edge[5].Z, edge[6].Z
                , 1, 1, 1, 1) * BezierPolygonMatrix;

            points.Add(p1 = new Vector4(p.M11, p.M21, p.M31, p.OffsetX).Normalized);
            points.Add(p2 = new Vector4(p.M12, p.M22, p.M32, p.OffsetY).Normalized);
            points.Add(p3 = new Vector4(p.M13, p.M23, p.M33, p.OffsetZ).Normalized);
            points.Add(p4 = new Vector4(p.M14, p.M24, p.M34, p.M44).Normalized);

            _points[(patchIndex + 1) % 3][19] = new PointEx(p1.X, p1.Y, p1.Z);
            _points[(patchIndex + 1) % 3][18] = new PointEx(p2.X, p2.Y, p2.Z);
            _points[(patchIndex + 1) % 3][17] = new PointEx(p3.X, p3.Y, p3.Z);
            _points[(patchIndex + 1) % 3][16] = new PointEx(p4.X, p4.Y, p4.Z);
            return points;
        }
        private List<Vector4> FindInnerPolygon(Vector4[] innerEdge)
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
            aaaa = aaaa.Union(points);

            return points;
        }
        #endregion Private Methods
        #region Public Methods
        public override void Draw()
        {
            Bitmap bmp = SceneManager.Instance.SceneImage;

            using (Graphics g = Graphics.FromImage(bmp))
            {
                Transform = Transformations.ViewMatrix(400);
                for (int j = 0; j < 3; j++)
                {

                    for (int i = 0; i < 20; i++)
                    {
                        if (_points[j][i] == null) continue;
                        var PointOnScreen = Transform * _points[j][i].PointOnScreen;
                        if (double.IsNaN(PointOnScreen.X) || double.IsNaN(PointOnScreen.Y) || PointOnScreen.X < 0 || PointOnScreen.X >= bmp.Width || PointOnScreen.Y < 0 || PointOnScreen.Y >= bmp.Height) return;
                        Color color = bmp.GetPixel((int)PointOnScreen.X, (int)PointOnScreen.Y);
                        if (i == 7 || i == 8 || i == 6 || i == 14)
                            g.FillRectangle(new SolidBrush(color.CombinedColor(Color.DeepPink)), (int)PointOnScreen.X, (int)PointOnScreen.Y, 3, 3);
                        else if (i == 1 || i == 2 || i == 12 || i == 13)
                            g.FillRectangle(new SolidBrush(color.CombinedColor(Color.Yellow)), (int)PointOnScreen.X, (int)PointOnScreen.Y, 3, 3);
                        else if (i == 3)
                            g.FillRectangle(new SolidBrush(color.CombinedColor(Color.White)), (int)PointOnScreen.X, (int)PointOnScreen.Y, 5, 5);
                        //else
                        //    g.FillRectangle(new SolidBrush(color.CombinedColor(Color.Lime)), (int)PointOnScreen.X, (int)PointOnScreen.Y, 3, 3);
                    }
                }
                //foreach (var point in aaaa)
                //{
                //    var PointOnScreen = Transform * point;
                //    if (double.IsNaN(PointOnScreen.X) || double.IsNaN(PointOnScreen.Y) || PointOnScreen.X < 0 || PointOnScreen.X >= bmp.Width || PointOnScreen.Y < 0 || PointOnScreen.Y >= bmp.Height) continue;
                //    Color color = bmp.GetPixel((int)PointOnScreen.X, (int)PointOnScreen.Y);
                //    g.FillRectangle(new SolidBrush(color.CombinedColor(Color.OrangeRed)), (int)PointOnScreen.X, (int)PointOnScreen.Y, 3, 3);
                //}
            }
            SceneManager.Instance.SceneImage = bmp;
        }
        #endregion Public Methods
    }
}

