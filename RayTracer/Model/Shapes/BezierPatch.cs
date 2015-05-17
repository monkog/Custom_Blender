using System.Collections.Generic;
using System.Linq;
using RayTracer.ViewModel;

namespace RayTracer.Model.Shapes
{
    public class BezierPatch : ModelBase
    {
        #region Protected Properties
        protected bool IsCylinder { get; private set; }
        protected const int BezierSegmentPoints = 3;
        protected int HorizontalPatches { get; private set; }
        protected int VerticalPatches { get; private set; }
        #endregion Protected Properties
        #region Public Properties
        public override IEnumerable<ShapeBase> SelectedItems { get { return Vertices.Where(p => p.IsSelected); } }
        #endregion Public Properties
        #region Constructors
        public BezierPatch(double x, double y, double z, string name)
            : base(x, y, z, name)
        {
            VerticalPatches = PatchManager.Instance.VerticalPatches;
            HorizontalPatches = PatchManager.Instance.HorizontalPatches;
            IsCylinder = PatchManager.Instance.IsCylinder;
        }
        #endregion Constructors
        #region Private Methods
        #endregion Private Methods
    }
}

