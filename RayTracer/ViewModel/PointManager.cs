using System.Collections.ObjectModel;
using RayTracer.Model.Shapes;

namespace RayTracer.ViewModel
{
    public class PointManager : ViewModelBase
    {
        #region Private Members
        /// <summary>
        /// Instance of PointManager
        /// </summary>
        private static PointManager _instance;
        private PointEx _selectedPoint;
        #endregion Private Members
        #region Public Properties
        /// <summary>
        /// Instance of the SceneManager
        /// </summary>
        public static PointManager Instance
        {
            get { return _instance ?? (_instance = new PointManager()); }
        }
        /// <summary>
        /// List of points on the screen
        /// </summary>
        public ObservableCollection<PointEx> Points { get; set; }
        /// <summary>
        /// Gets or sets the selected point.
        /// </summary>
        /// <value>
        /// The selected point.
        /// </value>
        public PointEx SelectedPoint
        {
            get { return _selectedPoint; }
            set
            {
                if (_selectedPoint == value) return;
                _selectedPoint = value;
                OnPropertyChanged("SelectedPoint");
            }
        }
        #endregion Public Properties
        #region Constructors
        /// <summary>
        /// Creates the new instance of PointManager
        /// </summary>
        private PointManager()
        {
            Points = new ObservableCollection<PointEx>();
        }
        #endregion Constructors
        #region Private Methods
        #endregion Private Methods
        #region Public Methods
        #endregion Public Methods
    }
}

