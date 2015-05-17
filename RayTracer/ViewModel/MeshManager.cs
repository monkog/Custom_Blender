using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RayTracer.Model.Shapes;

namespace RayTracer.ViewModel
{
    public class MeshManager : ViewModelBase
    {
        #region Private Members
        private static MeshManager _instance;
        private ObservableCollection<ModelBase> _meshes;
        #endregion Private Members
        #region Public Properties
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

