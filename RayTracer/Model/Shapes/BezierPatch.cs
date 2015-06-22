using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        protected double Width { get; private set; }
        protected double Height { get; private set; }
        protected Continuity Continuity { get; set; }
        #endregion Protected Properties
        #region Public Properties
        /// <summary>
        /// The patch points in an array
        /// </summary>
        public PointEx[,] Points;
        /// <summary>
        /// Gets a value indicating whether this patch is cylinder.
        /// </summary>
        public bool IsCylinder { get; private set; }
        public override IEnumerable<ShapeBase> SelectedItems { get { return Vertices.Where(p => p.IsSelected); } }
        /// <summary>
        /// The points common in patches.
        /// The point with a lower coordinate is the first point in the array
        /// </summary>
        public List<PointEx> CommonPoints;
        /// <summary>
        /// Gets the number of horizontal patches.
        /// </summary>
        public int HorizontalPatches { get; private set; }
        /// <summary>
        /// Gets the number of vertical patches.
        /// </summary>
        public int VerticalPatches { get; private set; }
        #endregion Public Properties
        #region Constructors
        protected BezierPatch(double x, double y, double z, string name, bool isCylinder, double width, double height
            , int verticalPatches, int horizontalPatches)
            : base(x, y, z, name)
        {
            VerticalPatches = verticalPatches;
            HorizontalPatches = horizontalPatches;
            IsCylinder = isCylinder;
            Width = width;
            Height = height;
            CommonPoints = new List<PointEx>();
            IsSelected = true;
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
        private int SortingFunction(PointEx x, PointEx y)
        {
            var xCoords = Points.CoordinatesOf(x);
            var yCoords = Points.CoordinatesOf(y);

            if ((xCoords.Item1 == 3 && yCoords.Item1 == 3) || (xCoords.Item2 == 0 && yCoords.Item2 == 0))
                return (xCoords.Item1 > yCoords.Item1 || xCoords.Item2 > yCoords.Item2) ? -1 : 1;
            return (xCoords.Item1 > yCoords.Item1 || xCoords.Item2 > yCoords.Item2) ? 1 : -1;
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
        protected abstract Vector4[,] GetPatchMatrix(int i, int j);
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
        /// <summary>
        /// Replaces the vertex.
        /// </summary>
        /// <param name="vertex">The vertex to replace.</param>
        /// <param name="interpolationPoint">The replacing vertex.</param>
        public void ReplaceVertex(PointEx vertex, PointEx interpolationPoint)
        {
            var index = Vertices.IndexOf(vertex);
            for (int i = 0; i < Points.GetLength(0); i++)
                for (int j = 0; j < Points.GetLength(1); j++)
                    if (Points[i, j] == vertex) Points[i, j] = interpolationPoint;
                    else Points[i, j].ModelTransform = ModelTransform * Points[i, j].ModelTransform;
            Vertices.RemoveAt(index);
            Vertices.Insert(index, interpolationPoint);
            CommonPoints.Add(interpolationPoint);
            ModelTransform = Matrix3D.Identity;

            if (CommonPoints.Count == 2)
                CommonPoints.Sort(SortingFunction);
        }
        /// <summary>
        /// Calculates the patch point depending on the points calculated from u and v.
        /// </summary>
        /// <param name="i">Horizontal patch index</param>
        /// <param name="j">Vertical patch index</param>
        /// <param name="u">Horizontal parametrization</param>
        /// <param name="v">Vertical parametrization</param>
        /// <returns>Point on screen from the patch</returns>
        public abstract Vector4 CalculatePatchPoint(int i, int j, double u, double v);
        #endregion Public Methods
    }
}

