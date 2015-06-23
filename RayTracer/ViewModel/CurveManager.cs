using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RayTracer.Model.Shapes;

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
    }
}

