using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using RayTracer.Helpers;
using RayTracer.Model.Shapes;
using Accord.Math;

namespace RayTracer.ViewModel
{
    public class CurveManager : ViewModelBase
    {
        #region Private Members
        private static CurveManager _instance;
        private ObservableCollection<BezierCurve> _curves;
        private ObservableCollection<TrimmingCurve> _trimmingCurves;
        #endregion Private Members
        #region Public Properties
        public ObservableCollection<BezierCurve> Curves
        {
            get { return _curves; }
            set
            {
                if (_curves == value)
                    return;
                _curves = value;
                OnPropertyChanged("Curves");
            }
        }
        public ObservableCollection<TrimmingCurve> TrimmingCurves
        {
            get { return _trimmingCurves; }
            set
            {
                if (_trimmingCurves == value)
                    return;
                _trimmingCurves = value;
                OnPropertyChanged("TrimmingCurves");
            }
        }
        /// <summary>
        /// Instance of the CurveManager
        /// </summary>
        public static CurveManager Instance
        {
            get { return _instance ?? (_instance = new CurveManager()); }
        }
        /// <summary>
        /// Gets or sets the selected Curves.
        /// </summary>
        public IEnumerable<BezierCurve> SelectedItems
        {
            get { return Curves.Where(c => c.IsSelected); }
        }
        #endregion Public Properties
        #region Constructor
        public CurveManager()
        {
            Curves = new ObservableCollection<BezierCurve>();
            TrimmingCurves = new ObservableCollection<TrimmingCurve>();
        }
        #endregion Constructor
        #region Public Methods
        /// <summary>
        /// Calculates the trimming curve for the last added pair of surfaces.
        /// </summary>
        /// <param name="mousePosition">The mouse click position.</param>
        public void CalculateTrimmingCurve(Point mousePosition)
        {
            var reverseTransform = SceneManager.Instance.TotalMatrix;
            reverseTransform.Invert();

            //var transform = SceneManager.Instance.TotalMatrix * Transformations.ViewMatrix(400);
            //double[,] transformMatrix = new[,] {{transform.M11, transform.M12, transform.M13, transform.M14}
            //                                  , {transform.M21, transform.M22, transform.M23, transform.M24}
            //                                  , {transform.M31, transform.M32, transform.M33, transform.M34}
            //                                  , {transform.OffsetX, transform.OffsetY, transform.OffsetZ, transform.M44}};
            //var reverse = transformMatrix.PseudoInverse();
            //var pos = new Vector4(mousePosition.X, mousePosition.Y, 0, 1) * reverse;


            var pos = reverseTransform * new Vector4(mousePosition.X, mousePosition.Y, 0, 1);

            var uv = FindSurfacePointIntersection(pos, TrimmingCurves.First().BezierPatches[0]);
            var st = FindSurfacePointIntersection(pos, TrimmingCurves.First().BezierPatches[1]);
        }
        #endregion
        #region Private Methods
        private Tuple<double, double> FindSurfacePointIntersection(Vector4 mousePosition, BezierPatch surface)
        {
            NewtonRhapsonPointSurface(mousePosition, surface, 0, 0);

            // 1. Calculate PseudoInverseMatrix
            // 2. Calculate MousePoint - Point on Patch from u,v
            // 3. Use Newton method
            // 4. Do the same for s,t
            // 5. Calculate PseudoInverse Jacobian Matrix for two patches
            // 6. Substract the point value of two patches
            // 7. Use Newton's method

            return null;
        }
        private void NewtonRhapsonPointSurface(Vector4 mousePosition, BezierPatch surface, int i, int j)
        {
            double epsilon = SceneManager.Epsilon;
            const int maxIterations = 20;

            double u = 0.5;
            double v = 0.5;
            Vector4 x1 = new Vector4(u, v, 0/*=t*/, 1);

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                var x = x1;
                var uPlus = surface.CalculatePatchPoint(i, j, x.Y + epsilon, x.Z);
                var uMinus = surface.CalculatePatchPoint(i, j, x.Y - epsilon, x.Z);
                var vPlus = surface.CalculatePatchPoint(i, j, x.Y, x.Z + epsilon);
                var vMinus = surface.CalculatePatchPoint(i, j, x.Y, x.Z - epsilon);
                var point = surface.CalculatePatchPoint(i, j, x.Y, x.Z);

                var jacobian = new[,]{{0.0, -(uPlus - uMinus).X/(2*epsilon), -(vPlus - vMinus).X/(2*epsilon)}
                                    , {0.0, -(uPlus - uMinus).Y/(2*epsilon), -(vPlus - vMinus).Y/(2*epsilon)}
                                    , {1.0, -(uPlus - uMinus).Z/(2*epsilon), -(vPlus - vMinus).Z/(2*epsilon)}};
                var inverse = jacobian.PseudoInverse();
                var error = new Vector4(mousePosition.X, mousePosition.Y, x.X, 1) - point;

                var dxByX = new Vector4(inverse[0, 0] * error.X + inverse[0, 1] * error.Y + inverse[0, 2] * error.Z
                                      , inverse[1, 0] * error.X + inverse[1, 1] * error.Y + inverse[1, 2] * error.Z
                                      , inverse[2, 0] * error.X + inverse[2, 1] * error.Y + inverse[2, 2] * error.Z
                                      , 1);
                x1 = x - dxByX;
                if (Math.Abs((x1 - x).X) + Math.Abs((x1 - x).Y) + Math.Abs((x1 - x).Z) < epsilon)
                    break;
            }
        }
        #endregion
    }
}

