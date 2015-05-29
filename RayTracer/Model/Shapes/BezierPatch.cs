using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
        protected Continuity Continuity { get; set; }
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
        }
        #endregion Constructors
        #region Private Methods
        private void SetCylinderVertices()
        {
            var manager = PatchManager.Instance;

            double topLeftY = Y - (manager.PatchHeight / 2);
            double radius = manager.PatchWidth;
            double alpha = (Math.PI * 2.0f) / Points.GetLength(1);
            double dy = manager.PatchHeight / Points.GetLength(0);

            for (int i = 0; i < Points.GetLength(0); i++)
                for (int j = 0; j < Points.GetLength(1); j++)
                {
                    var point = new PointEx(radius * Math.Cos(alpha * j), topLeftY + (i * dy), radius * Math.Sin(alpha * j));
                    Points[i, j] = point;
                    Vertices.Add(point);
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
            int verticalPoints = Points.GetLength(0);
            int horizontalPoints = Points.GetLength(1);

            for (int i = 0; i < verticalPoints; i++)
                for (int j = 0; j < horizontalPoints - 1; j++)
                    EdgesIndices.Add(new Tuple<int, int>(i * horizontalPoints + j
                        , i * horizontalPoints + j + 1));

            for (int i = 0; i < verticalPoints - 1; i++)
                for (int j = 0; j < horizontalPoints; j++)
                    EdgesIndices.Add(new Tuple<int, int>(i * horizontalPoints + j
                        , (i + 1) * horizontalPoints + j));

            CalculateShape();
        }
        private void SetCylinderEdges()
        {
            int verticalPoints = Points.GetLength(0);
            int horizontalPoints = Points.GetLength(1);

            for (int i = 0; i < verticalPoints; i++)
                for (int j = 0; j < horizontalPoints - 1; j++)
                    EdgesIndices.Add(new Tuple<int, int>(i * horizontalPoints + j, i * horizontalPoints + j + 1));

            for (int i = 0; i < verticalPoints; i++)
                EdgesIndices.Add(new Tuple<int, int>((i + 1) * horizontalPoints - 1, i * horizontalPoints));


            for (int i = 0; i < verticalPoints - 1; i++)
                for (int j = 0; j < horizontalPoints - 1; j++)
                    EdgesIndices.Add(new Tuple<int, int>(i * horizontalPoints + j, (i + 1) * horizontalPoints + j));

            CalculateShape();
        }
        #endregion Private Methods
        #region Protected Methods
        protected void SetVertices(PointEx[,] points, IEnumerable<PointEx> vertices, int verticalPoints, int horizontalPoints)
        {
            if (IsCylinder)
            {
                if (points == null)
                {
                    if (Continuity == Continuity.C2) Points = new PointEx[verticalPoints, horizontalPoints - 3];
                    else Points = new PointEx[verticalPoints, horizontalPoints - 1];
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
                    Points = new PointEx[verticalPoints, horizontalPoints];
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

