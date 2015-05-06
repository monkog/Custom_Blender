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
        }
        #endregion Constructor
    }
}

