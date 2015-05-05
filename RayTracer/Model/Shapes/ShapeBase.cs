using System.Windows.Media.Media3D;
using RayTracer.ViewModel;

namespace RayTracer.Model.Shapes
{
    public class ShapeBase : ViewModelBase
    {
        #region Private Members
        private bool _isSelected;
        private Matrix3D _transform;
        private Matrix3D _modelTransform;
        private string _name;
        #endregion Private Members
        #region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="Cube"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <param name="name">Name of the mesh</param>
        protected ShapeBase(double x, double y, double z, string name)
        {
            X = x;
            Y = y;
            Z = z;

            Name = name;
            IsSelected = false;
            _transform = Transformations.Identity;
            ModelTransform = Transformations.Identity;
        }
        #endregion .ctor
        #region Public Properties
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }
        /// <summary>
        /// Gets or sets the x position.
        /// </summary>
        /// <value>
        /// The x position.
        /// </value>
        public double X { get; set; }
        /// <summary>
        /// Gets or sets the y position.
        /// </summary>
        /// <value>
        /// The y position.
        /// </value>
        public double Y { get; set; }
        /// <summary>
        /// Gets or sets the z position.
        /// </summary>
        /// <value>
        /// The z position.
        /// </value>
        public double Z { get; set; }
        /// <summary>
        /// Gets or sets the transform multiplied by the projection transformations.
        /// </summary>
        /// <value>
        /// The transform multiplied by the projection transformations.
        /// </value>
        public Matrix3D Transform
        {
            get { return _transform; }
            set
            {
                _transform = SceneManager.Instance.TotalMatrix * value;
                CalculateShape();
            }
        }
        /// <summary>
        /// Gets or sets the current transform of the model.
        /// </summary>
        /// <value>
        /// The current transform of the model.
        /// </value>
        public Matrix3D ModelTransform
        {
            get { return _modelTransform; }
            set
            {
                if (_modelTransform == value)
                    return;
                _modelTransform = value;
                CalculateShape();
                OnPropertyChanged("ModelTransform");
            }
        }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        /// <summary>
        /// The thickness of a line drawing the shape
        /// </summary>
        protected int Thickness = 1;
        #endregion Public Properties
        #region Protected Methods
        /// <summary>
        /// Calculates the shape: vertices and edges
        /// </summary>
        protected virtual void CalculateShape()
        { }
        #endregion Protected Methods
        #region Private Methods
        #endregion Private Methods
        #region Public Methods
        #endregion Public Methods
    }
}

