using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using System.Windows.Media.Media3D;
using RayTracer.Helpers;
using RayTracer.ViewModel;

namespace RayTracer.Model.Shapes
{
    public class BezierPatchC0 : BezierPatch
    {
        #region Private Members
        private PointEx[,] _points;
        #endregion Private Members
        #region Public Properties
        public override string Type { get { return "BezierSurfaceC0"; } }
        #endregion Public Properties
        #region Constructors
        public BezierPatchC0(double x, double y, double z, string name, bool isCylinder, double width, double height
            , int verticalPatches, int horizontalPatches, PointEx[,] points = null, IEnumerable<PointEx> vertices = null)
            : base(x, y, z, name, isCylinder, width, height, verticalPatches, horizontalPatches)
        {
            SetVertices(points, vertices);
        }
        #endregion Constructors
        #region Private Methods
        private void DrawSinglePatch(Bitmap bmp, Graphics g, int patchIndex, int patchDivisions, Matrix3D matX, Matrix3D matY, Matrix3D matZ
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
        private void SetVertices(PointEx[,] points, IEnumerable<PointEx> vertices = null)
        {
            var manager = PatchManager.Instance;

            if (IsCylinder)
            {
                if (points == null)
                {
                    _points = new PointEx[manager.VerticalPatches * SceneManager.BezierSegmentPoints + 1, manager.HorizontalPatches * SceneManager.BezierSegmentPoints + 1];
                    SetCylinderVertices();
                }
                else
                {
                    _points = points;
                    Vertices = new ObservableCollection<PointEx>(vertices);
                    foreach (var vertex in vertices)
                        PointManager.Instance.Points.Add(vertex);
                }
                SetCylinderEdges();
            }
            else
            {
                if (points == null)
                {
                    _points = new PointEx[manager.VerticalPatches * SceneManager.BezierSegmentPoints + 1, manager.HorizontalPatches * SceneManager.BezierSegmentPoints + 1];
                    SetPlaneVertices();
                }
                else
                {
                    _points = points;
                    Vertices = new ObservableCollection<PointEx>(vertices);
                    foreach (var vertex in vertices)
                        PointManager.Instance.Points.Add(vertex);
                }
                SetPlaneEdges();
            }
        }
        private void SetPlaneVertices()
        {
            var manager = PatchManager.Instance;

            Vector3D topLeft = new Vector3D(X - (manager.PatchWidth / 2), Y - (manager.PatchHeight / 2), 0);
            double dx = manager.PatchWidth / (manager.HorizontalPatches * SceneManager.BezierSegmentPoints);
            double dy = manager.PatchHeight / (manager.VerticalPatches * SceneManager.BezierSegmentPoints);

            for (int i = 0; i < _points.GetLength(0); i++)
                for (int j = 0; j < _points.GetLength(1); j++)
                {
                    var point = new PointEx(topLeft.X + (j * dx), topLeft.Y + (i * dy), topLeft.Z) { CanBeDeleted = false };
                    _points[i, j] = point;
                    Vertices.Add(point);
                    PointManager.Instance.Points.Add(point);
                }
        }
        private void SetPlaneEdges()
        {
            const int bezierSegmentPoints = SceneManager.BezierSegmentPoints;

            for (int i = 0; i < VerticalPatches * bezierSegmentPoints + 1; i++)
                for (int j = 0; j < HorizontalPatches * bezierSegmentPoints; j++)
                    EdgesIndices.Add(new Tuple<int, int>(i * (HorizontalPatches * bezierSegmentPoints + 1) + j
                        , i * (HorizontalPatches * bezierSegmentPoints + 1) + j + 1));

            for (int i = 0; i < VerticalPatches * bezierSegmentPoints; i++)
                for (int j = 0; j < HorizontalPatches * bezierSegmentPoints + 1; j++)
                    EdgesIndices.Add(new Tuple<int, int>(i * (HorizontalPatches * bezierSegmentPoints + 1) + j
                        , (i + 1) * (HorizontalPatches * bezierSegmentPoints + 1) + j));

            CalculateShape();
        }
        private void SetCylinderVertices()
        {
            var manager = PatchManager.Instance;

            double topLeftY = Y - (manager.PatchHeight / 2);
            double radius = manager.PatchWidth;
            double alpha = (Math.PI * 2.0f) / (SceneManager.BezierSegmentPoints * manager.HorizontalPatches);
            double dy = manager.PatchHeight / (manager.VerticalPatches * SceneManager.BezierSegmentPoints);

            for (int i = 0; i < _points.GetLength(0); i++)
                for (int j = 0; j < _points.GetLength(1); j++)
                {
                    var point = new PointEx(radius * Math.Cos(alpha * j), topLeftY + (i * dy), radius * Math.Sin(alpha * j)) { CanBeDeleted = false };
                    _points[i, j] = point;
                    Vertices.Add(point);
                    PointManager.Instance.Points.Add(point);
                }
        }
        private void SetCylinderEdges()
        {
            const int bezierSegmentPoints = SceneManager.BezierSegmentPoints;

            for (int i = 0; i < PatchManager.Instance.VerticalPatches * bezierSegmentPoints + 1; i++)
                for (int j = 0; j < PatchManager.Instance.HorizontalPatches * bezierSegmentPoints; j++)
                    EdgesIndices.Add(new Tuple<int, int>(i * (PatchManager.Instance.HorizontalPatches * bezierSegmentPoints + 1) + j
                        , i * (PatchManager.Instance.HorizontalPatches * bezierSegmentPoints + 1) + j + 1));

            for (int i = 0; i < PatchManager.Instance.VerticalPatches * bezierSegmentPoints; i++)
                for (int j = 0; j < PatchManager.Instance.HorizontalPatches * bezierSegmentPoints; j++)
                    EdgesIndices.Add(new Tuple<int, int>(i * (PatchManager.Instance.HorizontalPatches * bezierSegmentPoints + 1) + j
                        , (i + 1) * (PatchManager.Instance.HorizontalPatches * bezierSegmentPoints + 1) + j));

            CalculateShape();
        }
        #endregion Private Methods
        #region Public Methods
        public override void Draw()
        {
            base.Draw();
            var manager = PatchManager.Instance;
            const int bezierSegmentPoints = SceneManager.BezierSegmentPoints;
            double maxX, maxY, minX, minY;
            _points.FindMaxMinCoords(out minX, out minY, out maxX, out maxY);

            var xDiv = 1.0 / ((maxX - minX) * 4);
            var yDiv = 1.0 / ((maxY - minY) * 4);

            Bitmap bmp = SceneManager.Instance.SceneImage;
            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < VerticalPatches; i++)
                {
                    for (int j = 0; j < HorizontalPatches; j++)
                    {
                        Matrix3D matX = new Matrix3D(_points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints].TransformedPosition.X, _points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 1].TransformedPosition.X, _points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 2].TransformedPosition.X, _points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 3].TransformedPosition.X
                                                   , _points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints].TransformedPosition.X, _points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 1].TransformedPosition.X, _points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 2].TransformedPosition.X, _points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 3].TransformedPosition.X
                                                   , _points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints].TransformedPosition.X, _points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 1].TransformedPosition.X, _points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 2].TransformedPosition.X, _points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 3].TransformedPosition.X
                                                   , _points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints].TransformedPosition.X, _points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 1].TransformedPosition.X, _points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 2].TransformedPosition.X, _points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 3].TransformedPosition.X);
                        Matrix3D matY = new Matrix3D(_points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints].TransformedPosition.Y, _points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 1].TransformedPosition.Y, _points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 2].TransformedPosition.Y, _points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 3].TransformedPosition.Y
                                                   , _points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints].TransformedPosition.Y, _points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 1].TransformedPosition.Y, _points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 2].TransformedPosition.Y, _points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 3].TransformedPosition.Y
                                                   , _points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints].TransformedPosition.Y, _points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 1].TransformedPosition.Y, _points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 2].TransformedPosition.Y, _points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 3].TransformedPosition.Y
                                                   , _points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints].TransformedPosition.Y, _points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 1].TransformedPosition.Y, _points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 2].TransformedPosition.Y, _points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 3].TransformedPosition.Y);
                        Matrix3D matZ = new Matrix3D(_points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints].TransformedPosition.Z, _points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 1].TransformedPosition.Z, _points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 2].TransformedPosition.Z, _points[i * bezierSegmentPoints + 0, j * bezierSegmentPoints + 3].TransformedPosition.Z
                                                   , _points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints].TransformedPosition.Z, _points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 1].TransformedPosition.Z, _points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 2].TransformedPosition.Z, _points[i * bezierSegmentPoints + 1, j * bezierSegmentPoints + 3].TransformedPosition.Z
                                                   , _points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints].TransformedPosition.Z, _points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 1].TransformedPosition.Z, _points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 2].TransformedPosition.Z, _points[i * bezierSegmentPoints + 2, j * bezierSegmentPoints + 3].TransformedPosition.Z
                                                   , _points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints].TransformedPosition.Z, _points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 1].TransformedPosition.Z, _points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 2].TransformedPosition.Z, _points[i * bezierSegmentPoints + 3, j * bezierSegmentPoints + 3].TransformedPosition.Z);

                        DrawSinglePatch(bmp, g, i, manager.VerticalPatchDivisions, matX, matY, matZ, Math.Max(xDiv, yDiv), isHorizontal: false);
                        DrawSinglePatch(bmp, g, j, manager.HorizontalPatchDivisions, matX, matY, matZ, Math.Max(xDiv, yDiv), isHorizontal: true);
                    }
                }
            }
            SceneManager.Instance.SceneImage = bmp;
        }
        public override void SaveControlPoints(StringBuilder stringBuilder)
        {
            foreach (var point in Vertices)
                stringBuilder.AppendLine("CP=" + point.Id);
        }
        #endregion Public Methods
    }
}

