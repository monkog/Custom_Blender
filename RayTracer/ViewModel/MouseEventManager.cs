using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RayTracer.Helpers.EventCommand;

namespace RayTracer.ViewModel
{
    public class MouseEventManager
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
        #endregion Private Members
        #region  Public Properties
        /// <summary>
        /// Gets the mouse previous position.
        /// </summary>
        /// <value>
        /// The mouse previous position.
        /// </value>
        public Point MousePreviousPosition { get; private set; }
        /// <summary>
        /// Gets the mouse current position.
        /// </summary>
        /// <value>
        /// The mouse current position.
        /// </value>
        public Point MouseCurrentPosition { get; private set; }
        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static MouseEventManager Instance { get { return _instance ?? (_instance = new MouseEventManager()); } }
        #endregion  Public Properties
        #region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="MouseEventManager"/> class.
        /// </summary>
        public MouseEventManager()
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
            MousePreviousPosition = args.GetPosition((Canvas)args.OriginalSource);
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
            if (!_isMouseDown) return;

            MouseCurrentPosition = args.GetPosition((Canvas)args.OriginalSource);
        }
        #endregion Commands
    }
}
