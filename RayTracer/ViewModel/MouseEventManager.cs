﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using RayTracer.Helpers;
using RayTracer.Helpers.EventCommand;
using RayTracer.Model;

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
        private const double Tolernce = 0.2;
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
            if (args.OriginalSource.GetType() == typeof(Canvas))
            {
                _mouseCurrentPosition = args.GetPosition((Canvas)args.OriginalSource);
                _isMouseDown = true;
            }
            else if (args.OriginalSource.GetType() == typeof(Image))
            {
                _mouseCurrentPosition = args.GetPosition((Image)args.OriginalSource);
                _isMouseDown = true;
            }
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
            if (!_isMouseDown || !(args.OriginalSource.GetType() == typeof(Canvas) || args.OriginalSource.GetType() == typeof(Image))) return;
            _mousePreviousPosition = _mouseCurrentPosition;
            if (args.OriginalSource.GetType() == typeof(Canvas))
                _mouseCurrentPosition = args.GetPosition((Canvas)args.OriginalSource);
            else if (args.OriginalSource.GetType() == typeof(Image))
                _mouseCurrentPosition = args.GetPosition((Image)args.OriginalSource);

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

        private ActionCommand<MouseButtonEventArgs> _rightMouseButtonCommand;
        public ActionCommand<MouseButtonEventArgs> RightMouseButtonCommand
        {
            get
            {
                return _rightMouseButtonCommand ??
                       (_rightMouseButtonCommand = new ActionCommand<MouseButtonEventArgs>(RightMouseButtonExecuted));
            }
        }
        /// <summary>
        /// Selects the 3D point
        /// </summary>
        private void RightMouseButtonExecuted(MouseButtonEventArgs args)
        {
            var position = args.GetPosition((IInputElement)args.Source);
            var reverseTransform = SceneManager.Instance.TransformMatrix * SceneManager.Instance.ScaleMatrix;
            reverseTransform.Invert();
            Vector4 pos = new Vector4(position.X, position.Y, 0, 1);
            pos = reverseTransform * pos;

            foreach (var point in PointManager.Instance.Points)
                if (point.X < pos.X + Tolernce && point.X > pos.X - Tolernce && point.Y < pos.Y + Tolernce && point.Y > pos.Y - Tolernce)
                {
                    PointManager.Instance.SelectedPoint = point;
                    var cursorPosition = new Vector3D(Cursor3D.Instance.XPosition, Cursor3D.Instance.YPosition, Cursor3D.Instance.ZPosition);
                    var delta = new Vector3D(point.X, point.Y, point.Z) - cursorPosition;
                    Cursor3D.Instance.ModelTransform = Transformations.TranslationMatrix(delta) * Cursor3D.Instance.ModelTransform;
                    return;
                }
        }
        #endregion Commands
    }
}
