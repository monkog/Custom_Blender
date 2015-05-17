using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.ViewModel;

namespace RayTracer.Model.Shapes
{
    public abstract class BezierPatch : ModelBase
    {
        #region Protected Properties
        protected double Width { get; private set; }
        protected double Height { get; private set; }
        protected bool IsCylinder { get; private set; }
        protected const int BezierSegmentPoints = 3;
        protected int HorizontalPatches { get; private set; }
        protected int VerticalPatches { get; private set; }
        #endregion Protected Properties
        #region Public Properties
        public override IEnumerable<ShapeBase> SelectedItems { get { return Vertices.Where(p => p.IsSelected); } }
        #endregion Public Properties
        #region Constructors
        protected BezierPatch(double x, double y, double z, string name)
            : base(x, y, z, name)
        {
            var patchManager = PatchManager.Instance;
            VerticalPatches = patchManager.VerticalPatches;
            HorizontalPatches = patchManager.HorizontalPatches;
            IsCylinder = patchManager.IsCylinder;
            Width = patchManager.PatchWidth;
            Height = patchManager.PatchHeight;
        }
        #endregion Constructors
        #region Public Methods
        public override void SaveParameters(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("Width=" + Width);
            stringBuilder.AppendLine("Height=" + Height);
            stringBuilder.AppendLine("PatchesXCount=" + HorizontalPatches);
            stringBuilder.AppendLine("PatchesYCount=" + VerticalPatches);
            stringBuilder.AppendLine("Cylindrical=" + IsCylinder);
        }
        #endregion Public Methods
    }
}

