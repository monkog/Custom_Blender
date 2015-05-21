using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RayTracer.Model.Shapes;

namespace RayTracer.ViewModel
{
    public class PatchManager : ViewModelBase
    {
        #region Private Members
        private static PatchManager _instance;
        private ObservableCollection<BezierPatch> _patches;
        private bool _isCylinder;
        private int _horizontalPatches;
        private int _verticalPatches;
        private int _horizontalPatchDivisions;
        private int _verticalPatchDivisions;
        private double _patchWidth;
        private double _patchHeight;
        private Continuity _patchContinuity;
        #endregion Private Members
        #region Public Properties
        /// <summary>
        /// Gets or sets a value indicating whether the newly created patch is cylinder.
        /// </summary>
        public bool IsCylinder
        {
            get { return _isCylinder; }
            set
            {
                if (_isCylinder == value) return;
                _isCylinder = value;
                OnPropertyChanged("IsCylinder");
            }
        }
        /// <summary>
        /// Gets or sets the number of the horizontal patches.
        /// </summary>
        public int HorizontalPatches
        {
            get { return _horizontalPatches; }
            set
            {
                if (_horizontalPatches == value) return;
                _horizontalPatches = value;
                OnPropertyChanged("HorizontalPatches");
            }
        }
        /// <summary>
        /// Gets or sets the number of the vertical patches.
        /// </summary>
        public int VerticalPatches
        {
            get { return _verticalPatches; }
            set
            {
                if (_verticalPatches == value) return;
                _verticalPatches = value;
                OnPropertyChanged("VerticalPatches");
            }
        }
        /// <summary>
        /// Gets or sets the number of the single horizontal patch divisions.
        /// </summary>
        public int HorizontalPatchDivisions
        {
            get { return _horizontalPatchDivisions; }
            set
            {
                if (_horizontalPatchDivisions == value) return;
                _horizontalPatchDivisions = value;
                OnPropertyChanged("HorizontalPatchDivisions");
            }
        }
        /// <summary>
        /// Gets or sets the number of the single vertical patch divisions.
        /// </summary>
        public int VerticalPatchDivisions
        {
            get { return _verticalPatchDivisions; }
            set
            {
                if (_verticalPatchDivisions == value) return;
                _verticalPatchDivisions = value;
                OnPropertyChanged("VerticalPatchDivisions");
            }
        }
        /// <summary>
        /// Gets or sets the width of the single patch.
        /// </summary>
        public double PatchWidth
        {
            get { return _patchWidth; }
            set
            {
                if (_patchWidth == value) return;
                _patchWidth = value;
                OnPropertyChanged("PatchWidth");
            }
        }
        /// <summary>
        /// Gets or sets the height of the single patch.
        /// </summary>
        public double PatchHeight
        {
            get { return _patchHeight; }
            set
            {
                if (_patchHeight == value) return;
                _patchHeight = value;
                OnPropertyChanged("PatchHeight");
            }
        }
        /// <summary>
        /// Gets or sets the continuity of the patch that will be created.
        /// </summary>
        public Continuity PatchContinuity
        {
            get { return _patchContinuity; }
            set
            {
                if (_patchContinuity == value) return;
                _patchContinuity = value;
                OnPropertyChanged("PatchContinuity");
            }
        }
        public ObservableCollection<BezierPatch> Patches
        {
            get { return _patches; }
            set
            {
                if (_patches == value)
                    return;
                _patches = value;
                OnPropertyChanged("Patches");
            }
        }
        /// <summary>
        /// Instance of the PatchManager
        /// </summary>
        public static PatchManager Instance
        {
            get { return _instance ?? (_instance = new PatchManager()); }
        }
        ///<summary>
        ///Gets or sets the selected Patches.
        ///</summary>
        public IEnumerable<BezierPatch> SelectedItems
        {
            get { return Patches.Where(p => p.IsSelected); }
        }
        #endregion Public Properties
        #region Constructor
        public PatchManager()
        {
            Patches = new ObservableCollection<BezierPatch>();
        }
        #endregion Constructor
    }
}

