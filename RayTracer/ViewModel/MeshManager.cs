using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RayTracer.Model.Shapes;

namespace RayTracer.ViewModel
{
    public class MeshManager : ViewModelBase
    {
        #region Private Members
        private double _smallR;
        private double _bigR;
        private int _l;
        private int _v;
        private static MeshManager _instance;
        private ObservableCollection<ModelBase> _meshes;
        #endregion Private Members
        #region Public Properties
        /// <summary>
        /// Gets or sets the small radius of the Torus.
        /// </summary>
        public double SmallR
        {
            get { return _smallR; }
            set
            {
                if (_smallR == value) return;
                _smallR = value;
                OnPropertyChanged("SmallR");
            }
        }
        /// <summary>
        /// Gets or sets the big radius of the Torus.
        /// </summary>
        public double BigR
        {
            get { return _bigR; }
            set
            {
                if (_bigR == value) return;
                _bigR = value;
                OnPropertyChanged("BigR");
            }
        }
        /// <summary>
        /// Gets or sets the number of torus donut divisions.
        /// </summary>
        public int L
        {
            get { return _l; }
            set
            {
                if (_l == value) return;
                _l = value;
                OnPropertyChanged("L");
            }
        }
        /// <summary>
        /// Gets or sets the torus circle divisions.
        /// </summary>
        public int V
        {
            get { return _v; }
            set
            {
                if (_v == value) return;
                _v = value;
                OnPropertyChanged("V");
            }
        }
        /// <summary>
        /// Gets or sets the collection of viewed meshes.
        /// </summary>
        /// <value>
        /// The collection of viewed meshes.
        /// </value>
        public ObservableCollection<ModelBase> Meshes
        {
            get { return _meshes; }
            set
            {
                if (_meshes == value)
                    return;
                _meshes = value;
                OnPropertyChanged("Meshes");
            }
        }
        /// <summary>
        /// Instance of the MeshManager
        /// </summary>
        public static MeshManager Instance
        {
            get { return _instance ?? (_instance = new MeshManager()); }
        }
        ///<summary>
        ///Gets or sets the selected Meshes.
        ///</summary>
        public IEnumerable<ModelBase> SelectedItems
        {
            get { return Meshes.Where(p => p.IsSelected); }
        }
        #endregion Public Properties
        #region Constructors
        public MeshManager()
        {
            Meshes = new ObservableCollection<ModelBase>();
        }
        #endregion Constructors
    }
}

