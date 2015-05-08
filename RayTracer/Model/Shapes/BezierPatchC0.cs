using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using RayTracer.ViewModel;

namespace RayTracer.Model.Shapes
{
    public class BezierPatchC0 : ModelBase
    {
        #region Private Members
        private bool _isCylinder;
        private double[,] _x;
        private double[,] _y;
        private double[,] _z;
        private const int BezierSegmentPoints = 3;
        #endregion Private Members
        #region Public Properties
        public IEnumerable<object> SelectedItems { get { return Vertices.Where(p => p.IsSelected); } }
        #endregion Public Properties
        #region Constructors
        public BezierPatchC0(double x, double y, double z, string name)
            : base(x, y, z, name)
        {
            _isCylinder = PatchManager.Instance.IsCylinder;
            SetVertices();
        }
        #endregion Constructors
        #region Private Methods
        /// <summary>
        /// Sets the vertices.
        /// </summary>
        private void SetVertices()
        {
            if (_isCylinder)
            {
                SetCylinderVertices();
                SetCylinderEdges();
            }
            else
            {
                SetPlaneVertices();
                SetPlaneEdges();
            }
        }
        private void SetPlaneVertices()
        {
            var manager = PatchManager.Instance;
            _x = new double[manager.VerticalPatches * BezierSegmentPoints + 1, manager.HorizontalPatches * BezierSegmentPoints + 1];
            _y = new double[manager.VerticalPatches * BezierSegmentPoints + 1, manager.HorizontalPatches * BezierSegmentPoints + 1];
            _z = new double[manager.VerticalPatches * BezierSegmentPoints + 1, manager.HorizontalPatches * BezierSegmentPoints + 1];

            Vector3D topLeft = new Vector3D(X - (manager.PatchWidth / 2), Y - (manager.PatchHeight / 2), 0);
            double dx = manager.PatchWidth / (manager.HorizontalPatches * BezierSegmentPoints);
            double dy = manager.PatchHeight / (manager.VerticalPatches * BezierSegmentPoints);

            for (int i = 0; i < _x.GetLength(0); i++)
                for (int j = 0; j < _x.GetLength(1); j++)
                {
                    var point = new PointEx(topLeft.X + (j * dx), topLeft.Y + (i * dy), topLeft.Z);
                    _x[i, j] = point.X;
                    _y[i, j] = point.Y;
                    _z[i, j] = point.Z;
                    Vertices.Add(point);
                }
        }
        private void SetPlaneEdges()
        {
            for (int i = 0; i < PatchManager.Instance.VerticalPatches * BezierSegmentPoints + 1; i++)
                for (int j = 0; j < PatchManager.Instance.HorizontalPatches * BezierSegmentPoints; j++)
                    EdgesIndices.Add(new Tuple<int, int>(i * ((PatchManager.Instance.VerticalPatches + 1) * BezierSegmentPoints + 1) + j
                        , i * ((PatchManager.Instance.VerticalPatches + 1) * BezierSegmentPoints + 1) + j + 1));

            for (int i = 0; i < PatchManager.Instance.VerticalPatches * BezierSegmentPoints; i++)
                for (int j = 0; j < PatchManager.Instance.HorizontalPatches * BezierSegmentPoints + 1; j++)
                    EdgesIndices.Add(new Tuple<int, int>(i * ((PatchManager.Instance.VerticalPatches + 1) * BezierSegmentPoints + 1) + j
                        , (i + 1) * ((PatchManager.Instance.VerticalPatches + 1) * BezierSegmentPoints + 1) + j));

            CalculateShape();
        }
        private void SetCylinderVertices()
        {
            var manager = PatchManager.Instance;
            _x = new double[manager.VerticalPatches * BezierSegmentPoints + 1, manager.HorizontalPatches * BezierSegmentPoints + 1];
            _y = new double[manager.VerticalPatches * BezierSegmentPoints + 1, manager.HorizontalPatches * BezierSegmentPoints + 1];
            _z = new double[manager.VerticalPatches * BezierSegmentPoints + 1, manager.HorizontalPatches * BezierSegmentPoints + 1];

            double topLeftY = Y - (manager.PatchHeight / 2);
            double radius = manager.PatchWidth;
            double alpha = (Math.PI * 2.0f) / (BezierSegmentPoints * manager.HorizontalPatches);
            double dy = manager.PatchHeight / (manager.VerticalPatches * BezierSegmentPoints);

            for (int i = 0; i < _x.GetLength(0); i++)
                for (int j = 0; j < _x.GetLength(1); j++)
                {
                    var point = new PointEx(radius * Math.Cos(alpha * j), topLeftY + (i * dy), radius * Math.Sin(alpha * j));
                    _x[i, j] = point.X;
                    _y[i, j] = point.Y;
                    _z[i, j] = point.Z;
                    Vertices.Add(point);
                }
        }
        private void SetCylinderEdges()
        {
            for (int i = 0; i < PatchManager.Instance.VerticalPatches * BezierSegmentPoints + 1; i++)
                for (int j = 0; j < PatchManager.Instance.HorizontalPatches * BezierSegmentPoints; j++)
                    EdgesIndices.Add(new Tuple<int, int>(i * ((PatchManager.Instance.VerticalPatches + 1) * BezierSegmentPoints + 1) + j
                        , i * ((PatchManager.Instance.VerticalPatches + 1) * BezierSegmentPoints + 1) + j + 1));

            for (int i = 0; i < PatchManager.Instance.VerticalPatches * BezierSegmentPoints; i++)
                for (int j = 0; j < PatchManager.Instance.HorizontalPatches * BezierSegmentPoints; j++)
                    EdgesIndices.Add(new Tuple<int, int>(i * ((PatchManager.Instance.VerticalPatches + 1) * BezierSegmentPoints + 1) + j
                        , (i + 1) * ((PatchManager.Instance.VerticalPatches + 1) * BezierSegmentPoints + 1) + j));

            CalculateShape();
        }
        private double BezierBlend(int k, double mu, int n)
        {
            int nn, kn, nkn;
            double blend = 1;

            nn = n;
            kn = k;
            nkn = n - k;

            while (nn >= 1)
            {
                blend *= nn;
                nn--;
                if (kn > 1)
                {
                    blend /= (double)kn;
                    kn--;
                }
                if (nkn > 1)
                {
                    blend /= (double)nkn;
                    nkn--;
                }
            }
            if (k > 0)
                blend *= Math.Pow(mu, (double)k);
            if (n - k > 0)
                blend *= Math.Pow(1 - mu, (double)(n - k));

            return (blend);
        }
        #endregion Private Methods
        #region Public Methods
        public override void Draw()
        {
            var manager = PatchManager.Instance;
            var points = new PointEx[(manager.VerticalPatches * (manager.VerticalPatchDivisions - 1)) + 1, (manager.HorizontalPatches * (manager.HorizontalPatchDivisions - 1)) + 1];

            for (int i = 0; i < manager.VerticalPatches; i++)
            {
                for (int j = 0; j < manager.HorizontalPatches; j++)
                {
                    for (int k = 0; k < manager.VerticalPatchDivisions; k++)
                    {
                        for (int l = 0; l < manager.HorizontalPatchDivisions; l++)
                        {
                            for (int m = 0; m < 100; m++)
                            {

                            }
                        }
                    }
                }
            }
            base.Draw();
        }
        #endregion Public Methods
    }
}

