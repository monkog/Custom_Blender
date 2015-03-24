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

