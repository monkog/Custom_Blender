using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using RayTracer.ViewModel;

namespace RayTracer.Model.Shapes
{
    public class TrimmingCurve : ModelBase
    {
        #region Private Members
        #endregion Private Members
        #region Public Properties
        public override string Type
        {
            get { return "TrimmedCurve"; }
        }
        public override IEnumerable<ShapeBase> SelectedItems
        {
            get { return null; }
        }
        /// <summary>
        /// Two patches for calculating the trimming curve
        /// </summary>
        public BezierPatch[] BezierPatches { get; private set; }
        #endregion Public Properties
        #region Constructors
        public TrimmingCurve(double x, double y, double z, string name, List<BezierPatch> patches)
            : base(x, y, z, name)
        {
            Bitmap bmp = SceneManager.Instance.FirstSurfaceImage;
            using (Graphics g = Graphics.FromImage(SceneManager.Instance.FirstSurfaceImage))
            {
                g.Clear(Color.Black);
            }
            SceneManager.Instance.FirstSurfaceImage = bmp;

            bmp = SceneManager.Instance.SecondSurfaceImage;
            using (Graphics g = Graphics.FromImage(SceneManager.Instance.SecondSurfaceImage))
            {
                g.Clear(Color.Black);
            }
            SceneManager.Instance.SecondSurfaceImage = bmp;
            MouseEventManager.Instance.CaptureNewtonStartPoint = true;
            BezierPatches = new[] { patches.ElementAt(0), patches.ElementAt(1) };
        }
        #endregion Constructors
        #region Private Methods
        #endregion Private Methods
        #region Public Methods
        #endregion Public Methods
    }
}

