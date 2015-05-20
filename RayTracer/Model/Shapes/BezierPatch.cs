using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using RayTracer.Helpers;
using RayTracer.ViewModel;

namespace RayTracer.Model.Shapes
{
    public abstract class BezierPatch : ModelBase
    {
        #region Protected Properties
        protected PointEx[,] Points;
        protected double Width { get; private set; }
        protected double Height { get; private set; }
        protected int HorizontalPatches { get; private set; }
        protected int VerticalPatches { get; private set; }
        #endregion Protected Properties
        #region Public Properties
        /// <summary>
        /// Gets a value indicating whether this patch is cylinder.
        /// </summary>
        public bool IsCylinder { get; private set; }
        public override IEnumerable<ShapeBase> SelectedItems { get { return Vertices.Where(p => p.IsSelected); } }
        #endregion Public Properties
        #region Constructors
        protected BezierPatch(double x, double y, double z, string name, bool isCylinder, double width, double height
            , int verticalPatches, int horizontalPatches, PointEx[,] points, IEnumerable<PointEx> vertices)
            : base(x, y, z, name)
        {
            VerticalPatches = verticalPatches;
            HorizontalPatches = horizontalPatches;
            IsCylinder = isCylinder;
            Width = width;
            Height = height;
            SetVertices(points, vertices);
        }
        #endregion Constructors
        #region Private Methods
        private void SetVertices(PointEx[,] points, IEnumerable<PointEx> vertices)
        {
            var manager = PatchManager.Instance;

            if (IsCylinder)
            {
                if (points == null)
                {
                    Points = new PointEx[manager.VerticalPatches * SceneManager.BezierSegmentPoints + 1, manager.HorizontalPatches * SceneManager.BezierSegmentPoints + 1];
                    SetCylinderVertices();
                }
                else
                {
                    Points = points;
                    Vertices = new ObservableCollection<PointEx>(vertices);
                }
                SetCylinderEdges();
            }
            else
            {
                if (points == null)
                {
                    Points = new PointEx[manager.VerticalPatches * SceneManager.BezierSegmentPoints + 1, manager.HorizontalPatches * SceneManager.BezierSegmentPoints + 1];
                    SetPlaneVertices();
                }
                else
                {
                    Points = points;
                    Vertices = new ObservableCollection<PointEx>(vertices);
                }
                SetPlaneEdges();
            }
        }
        private void SetPlaneVertices()
        {
            var manager = PatchManager.Instance;

            Vector4 topLeft = new Vector4(X - (manager.PatchWidth / 2), Y - (manager.PatchHeight / 2), Cursor3D.Instance.ZPosition, 1);
            double dx = manager.PatchWidth / (manager.HorizontalPatches * SceneManager.BezierSegmentPoints);
            double dy = manager.PatchHeight / (manager.VerticalPatches * SceneManager.BezierSegmentPoints);

            for (int i = 0; i < Points.GetLength(0); i++)
                for (int j = 0; j < Points.GetLength(1); j++)
                {
                    var point = new PointEx(topLeft.X + (j * dx), topLeft.Y + (i * dy), topLeft.Z);
                    Points[i, j] = point;
                    Vertices.Add(point);
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

            for (int i = 0; i < Points.GetLength(0); i++)
                for (int j = 0; j < Points.GetLength(1); j++)
                {
                    var point = new PointEx(radius * Math.Cos(alpha * j), topLeftY + (i * dy), radius * Math.Sin(alpha * j));
                    Points[i, j] = point;
                    Vertices.Add(point);
                }
        }
        private void SetCylinderEdges()
        {
            const int bezierSegmentPoints = SceneManager.BezierSegmentPoints;

            for (int i = 0; i < VerticalPatches * bezierSegmentPoints + 1; i++)
                for (int j = 0; j < HorizontalPatches * bezierSegmentPoints; j++)
                    EdgesIndices.Add(new Tuple<int, int>(i * (HorizontalPatches * bezierSegmentPoints + 1) + j
                        , i * (HorizontalPatches * bezierSegmentPoints + 1) + j + 1));

            for (int i = 0; i < VerticalPatches * bezierSegmentPoints; i++)
                for (int j = 0; j < HorizontalPatches * bezierSegmentPoints; j++)
                    EdgesIndices.Add(new Tuple<int, int>(i * (HorizontalPatches * bezierSegmentPoints + 1) + j
                        , (i + 1) * (HorizontalPatches * bezierSegmentPoints + 1) + j));

            CalculateShape();
        }
        #endregion Private Methods
        #region Protected Properties
        protected abstract void DrawSinglePatch(Bitmap bmp, Graphics g, int patchIndex, int patchDivisions, Matrix3D matX, Matrix3D matY, Matrix3D matZ
            , double divisions, bool isHorizontal);
        #endregion Protected Properties
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
        public override void SaveControlPoints(StringBuilder stringBuilder)
        {
            foreach (var point in Vertices)
                point.Save(stringBuilder);
        }
        public override void SaveParameters(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("Width=" + Width);
            stringBuilder.AppendLine("Height=" + Height);
            stringBuilder.AppendLine("PatchesXCount=" + HorizontalPatches);
            stringBuilder.AppendLine("PatchesYCount=" + VerticalPatches);
            stringBuilder.AppendLine("Cylindrical=" + IsCylinder);
        }
        public override void SaveControlPointsReference(StringBuilder stringBuilder)
        {
            foreach (var point in Vertices)
                stringBuilder.AppendLine("CP=" + point.Id);
        }
        #endregion Public Methods
    }
}

