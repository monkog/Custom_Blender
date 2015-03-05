using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using RayTracer.Helpers.EventCommand;

namespace RayTracer.ViewModel
{
    public class MouseEventManager : ViewModelBase
    {
        #region Private Members
        /// <summary>
        /// The instance of the mouse manager
        /// </summary>
        private static MouseEventManager _instance;
        /// <summary>
        /// Is mouse down
        /// </summary>
        private bool _isMouseDown;
        /// <value>
        /// The mouse previous position.
        /// </value>
        private Point _mousePreviousPosition;
        /// <value>
        /// The mouse current position.
        /// </value>
        private Point _mouseCurrentPosition;
        /// <summary>
        /// The mouse delta
        /// </summary>
        private Point _mouseDelta;
        /// <summary>
        /// The mouse scale
        /// </summary>
        private double _mouseScale;
        #endregion Private Members
        #region  Public Properties
        /// <summary>
        /// Gets or sets the mouse delta.
        /// </summary>
        /// <value>
        /// The mouse delta.
        /// </value>
        public Point MouseDelta
        {
            get { return _mouseDelta; }
            set
            {
                if (_mouseDelta == value) return;
                _mouseDelta = value;
                OnPropertyChanged("MouseDelta");
            }
        }
        /// <summary>
        /// Gets or sets the mouse scale.
        /// </summary>
        /// <value>
        /// The mouse scale.
        /// </value>
        public double MouseScale
        {
            get { return _mouseScale; }
            set
            {
                _mouseScale = value;
                OnPropertyChanged("MouseScale");
            }
        }
        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static MouseEventManager Instance { get { return _instance ?? (_instance = new MouseEventManager()); } }
        #endregion  Public Properties
        #region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="MouseEventManager"/> class.
        /// </summary>
        private MouseEventManager()
        {
            _isMouseDown = false;
        }
        #endregion .ctor
        #region Commands
        private ActionCommand<MouseButtonEventArgs> _mouseClickCommand;
        public ActionCommand<MouseButtonEventArgs> MouseClickCommand
        {
            get
            {
                return _mouseClickCommand ??
                       (_mouseClickCommand = new ActionCommand<MouseButtonEventArgs>(MouseClickExecuted));
            }
        }
        /// <summary>
        /// Captures mouse position
        /// </summary>
        private void MouseClickExecuted(MouseButtonEventArgs args)
        {
            if (args.OriginalSource.GetType() != typeof(Canvas)) return;
            _mouseCurrentPosition = args.GetPosition((Canvas)args.OriginalSource);
            _isMouseDown = true;
        }

        private ActionCommand<MouseButtonEventArgs> _mouseUpCommand;
        public ActionCommand<MouseButtonEventArgs> MouseUpCommand
        {
            get
            {
                return _mouseUpCommand ??
                       (_mouseUpCommand = new ActionCommand<MouseButtonEventArgs>(MouseUpExecuted));
            }
        }
        /// <summary>
        /// Captures mouse position
        /// </summary>
        private void MouseUpExecuted(MouseButtonEventArgs args)
        {
            _isMouseDown = false;
        }

        private ActionCommand<MouseEventArgs> _mouseMoveCommand;
        public ActionCommand<MouseEventArgs> MouseMoveCommand
        {
            get
            {
                return _mouseMoveCommand ??
                       (_mouseMoveCommand = new ActionCommand<MouseEventArgs>(MouseMoveExecuted));
            }
        }
        /// <summary>
        /// Updates mouse position
        /// </summary>
        private void MouseMoveExecuted(MouseEventArgs args)
        {
            if (!_isMouseDown || args.OriginalSource.GetType() != typeof(Canvas)) return;
            _mousePreviousPosition = _mouseCurrentPosition;
            _mouseCurrentPosition = args.GetPosition((Canvas)args.OriginalSource);

#warning move constants
            MouseDelta = new Point((_mouseCurrentPosition.X - _mousePreviousPosition.X) / 50, (_mouseCurrentPosition.Y - _mousePreviousPosition.Y) / 50);
        }

        private ActionCommand<MouseWheelEventArgs> _mouseWheelCommand;
        public ActionCommand<MouseWheelEventArgs> MouseWheelCommand
        {
            get
            {
                return _mouseWheelCommand ??
                       (_mouseWheelCommand = new ActionCommand<MouseWheelEventArgs>(MouseWheelExecuted));
            }
        }
        /// <summary>
        /// Zooms in/out the viewport
        /// </summary>
        private void MouseWheelExecuted(MouseWheelEventArgs args)
        {
            MouseScale = 1 + args.Delta / 1000f;
        }
        #endregion Commands
    }
}
