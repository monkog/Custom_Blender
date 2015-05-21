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
        #region Protected Methods
        #endregion Protected Methods
        #region Public Methods
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

