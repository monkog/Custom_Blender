using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media.Media3D;
using RayTracer.Helpers;
using RayTracer.ViewModel;

namespace RayTracer.Model.Shapes
{
    public class BezierPatchC2 : BezierPatch
    {
        #region Private Members
        private double[] _knots;
        private double[,] _nMatrix;
        #endregion Private Members
        #region Public Properties
        public override string Type { get { return "BezierSurfaceC2"; } }
        #endregion Public Properties
        #region Constructors
        public BezierPatchC2(double x, double y, double z, string name, bool isCylinder, double width, double height
            , int verticalPatches, int horizontalPatches, PointEx[,] points = null, IEnumerable<PointEx> vertices = null)
            : base(x, y, z, name, isCylinder, width, height, verticalPatches, horizontalPatches, points, vertices)
        {
            const int n = SceneManager.BezierSegmentPoints;
            _knots = new double[Vertices.Count + n + 4];
            double interval = 1 / (double)(Vertices.Count + n + 3);

            for (int i = 0; i < Vertices.Count + n + 4; i++)
                _knots[i] = i * interval;

            _nMatrix = _knots.CalculateNMatrix(n, Vertices.Count);
        }
        #endregion Constructors
        #region Private Methods
        private void SetSplineKnots(int n)
        {
            _knots = new double[n + SceneManager.BezierSegmentPoints + 4];

            double interval = 1 / (double)(n + SceneManager.BezierSegmentPoints + 3);

            for (int i = 0; i < n + SceneManager.BezierSegmentPoints + 4; i++)
                _knots[i] = i * interval;
        }
        #endregion Private Methods
        #region Protected Methods
        protected override void DrawSinglePatch(Bitmap bmp, Graphics g, int patchIndex, int patchDivisions, Matrix3D matX, Matrix3D matY, Matrix3D matZ
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
        #endregion Protected Methods
    }
}

