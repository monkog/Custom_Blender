using System.Collections.ObjectModel;
using RayTracer.ViewModel;
namespace RayTracer.Model.Shapes
{
    public class BezierPatchC2 : BezierPatch
    {
        #region Private Members
        private bool _isBernsteinBasis;
        #endregion Private Members
        #region Public Properties
        /// <summary>
        /// Vertices for the B-Spline basis.
        /// </summary>
        public PointEx[,] DeBooreVertices { get; private set; }
        /// <summary>
        /// Vertices for the Bernstein basis.
        /// </summary>
        public PointEx[,] BezierVertices { get; private set; }
        /// <summary>
        /// Determines whether the Bezier patch is being displayed using Bernstein basis.
        /// If not, it's in B-Spline basis.
        /// </summary>
        public bool IsBernsteinBasis
        {
            get { return _isBernsteinBasis; }
            set
            {
                if (_isBernsteinBasis == value) return;
                _isBernsteinBasis = value;
                //SetVertices();
                OnPropertyChanged("IsBernsteinBasis");
            }
        }
        #endregion Public Properties
        #region Constructors
        public BezierPatchC2(double x, double y, double z, string name) : base(x, y, z, name)
        {
            //SetVertices();
        }
        #endregion Constructors
        #region Private Methods
        //private void SetVertices()
        //{
        //    var manager = PatchManager.Instance;
        //    DeBooreVertices = new PointEx[manager.VerticalPatches * BezierSegmentPoints + 1, manager.HorizontalPatches * BezierSegmentPoints + 1];
        //    IsBernsteinBasis = true;

        //    if (IsCylinder)
        //    {
        //        SetCylinderVertices();
        //        SetCylinderEdges();
        //    }
        //    else
        //    {
        //        SetPlaneVertices();
        //        SetPlaneEdges();
        //    }
        //}
        //private void DrawSinglePatch(Bitmap bmp, Graphics g, int patchIndex, int patchDivisions, Matrix3D matX, Matrix3D matY, Matrix3D matZ
        //    , double divisions, bool isHorizontal)
        //{
        //    double step = 1.0f / (patchDivisions - 1);
        //    double currentStep = patchIndex == 0 ? 0 : step;
        //    Vector4 pointX = null, pointY = null;

        //    for (double m = (patchIndex == 0 ? 0 : 1); m < patchDivisions; m++, currentStep += step)
        //    {
        //        if (isHorizontal)
        //            pointY = new Vector4(Math.Pow((1.0 - currentStep), 3), 3 * currentStep * Math.Pow((1.0 - currentStep), 2), 3 * currentStep * currentStep * (1.0 - currentStep), Math.Pow(currentStep, 3));
        //        else
        //            pointX = new Vector4(Math.Pow((1.0 - currentStep), 3), 3 * currentStep * Math.Pow((1.0 - currentStep), 2), 3 * currentStep * currentStep * (1.0 - currentStep), Math.Pow(currentStep, 3));

        //        for (double n = 0; n <= 1; n += divisions)
        //        {
        //            if (isHorizontal)
        //                pointX = new Vector4(Math.Pow((1.0 - n), 3), 3 * n * Math.Pow((1.0 - n), 2), 3 * n * n * (1.0 - n), Math.Pow(n, 3));
        //            else
        //                pointY = new Vector4(Math.Pow((1.0 - n), 3), 3 * n * Math.Pow((1.0 - n), 2), 3 * n * n * (1.0 - n), Math.Pow(n, 3));

        //            var x = pointX * matX * pointY;
        //            var y = pointX * matY * pointY;
        //            var z = pointX * matZ * pointY;
        //            SceneManager.DrawCurvePoint(bmp, g, new Vector4(x, y, z, 1), Thickness);
        //        }
        //    }
        //}
        //private void SetVertices()
        //{
        //    var manager = PatchManager.Instance;
        //    _points = new PointEx[manager.VerticalPatches * BezierSegmentPoints + 1, manager.HorizontalPatches * BezierSegmentPoints + 1];

        //    if (IsCylinder)
        //    {
        //        SetCylinderVertices();
        //        SetCylinderEdges();
        //    }
        //    else
        //    {
        //        SetPlaneVertices();
        //        SetPlaneEdges();
        //    }
        //}
        //private void SetPlaneVertices()
        //{
        //    var manager = PatchManager.Instance;

        //    Vector3D topLeft = new Vector3D(X - (manager.PatchWidth / 2), Y - (manager.PatchHeight / 2), 0);
        //    double dx = manager.PatchWidth / (manager.HorizontalPatches * BezierSegmentPoints);
        //    double dy = manager.PatchHeight / (manager.VerticalPatches * BezierSegmentPoints);

        //    for (int i = 0; i < _points.GetLength(0); i++)
        //        for (int j = 0; j < _points.GetLength(1); j++)
        //        {
        //            var point = new PointEx(topLeft.X + (j * dx), topLeft.Y + (i * dy), topLeft.Z);
        //            _points[i, j] = point;
        //            Vertices.Add(point);
        //        }
        //}
        //private void SetPlaneEdges()
        //{
        //    for (int i = 0; i < PatchManager.Instance.VerticalPatches * BezierSegmentPoints + 1; i++)
        //        for (int j = 0; j < PatchManager.Instance.HorizontalPatches * BezierSegmentPoints; j++)
        //            EdgesIndices.Add(new Tuple<int, int>(i * (PatchManager.Instance.HorizontalPatches * BezierSegmentPoints + 1) + j
        //                , i * (PatchManager.Instance.HorizontalPatches * BezierSegmentPoints + 1) + j + 1));

        //    for (int i = 0; i < PatchManager.Instance.VerticalPatches * BezierSegmentPoints; i++)
        //        for (int j = 0; j < PatchManager.Instance.HorizontalPatches * BezierSegmentPoints + 1; j++)
        //            EdgesIndices.Add(new Tuple<int, int>(i * (PatchManager.Instance.HorizontalPatches * BezierSegmentPoints + 1) + j
        //                , (i + 1) * (PatchManager.Instance.HorizontalPatches * BezierSegmentPoints + 1) + j));

        //    CalculateShape();
        //}
        //private void SetCylinderVertices()
        //{
        //    var manager = PatchManager.Instance;

        //    double topLeftY = Y - (manager.PatchHeight / 2);
        //    double radius = manager.PatchWidth;
        //    double alpha = (Math.PI * 2.0f) / (BezierSegmentPoints * manager.HorizontalPatches);
        //    double dy = manager.PatchHeight / (manager.VerticalPatches * BezierSegmentPoints);

        //    for (int i = 0; i < _points.GetLength(0); i++)
        //        for (int j = 0; j < _points.GetLength(1); j++)
        //        {
        //            var point = new PointEx(radius * Math.Cos(alpha * j), topLeftY + (i * dy), radius * Math.Sin(alpha * j));
        //            _points[i, j] = point;
        //            Vertices.Add(point);
        //        }
        //}
        //private void SetCylinderEdges()
        //{
        //    for (int i = 0; i < PatchManager.Instance.VerticalPatches * BezierSegmentPoints + 1; i++)
        //        for (int j = 0; j < PatchManager.Instance.HorizontalPatches * BezierSegmentPoints; j++)
        //            EdgesIndices.Add(new Tuple<int, int>(i * (PatchManager.Instance.HorizontalPatches * BezierSegmentPoints + 1) + j
        //                , i * (PatchManager.Instance.HorizontalPatches * BezierSegmentPoints + 1) + j + 1));

        //    for (int i = 0; i < PatchManager.Instance.VerticalPatches * BezierSegmentPoints; i++)
        //        for (int j = 0; j < PatchManager.Instance.HorizontalPatches * BezierSegmentPoints; j++)
        //            EdgesIndices.Add(new Tuple<int, int>(i * (PatchManager.Instance.HorizontalPatches * BezierSegmentPoints + 1) + j
        //                , (i + 1) * (PatchManager.Instance.HorizontalPatches * BezierSegmentPoints + 1) + j));

        //    CalculateShape();
        //}
        #endregion Private Methods
        #region Public Methods
        #endregion Public Methods
    }
}

