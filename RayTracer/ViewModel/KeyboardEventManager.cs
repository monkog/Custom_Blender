using System.Windows.Input;
using System.Windows.Media.Media3D;
using RayTracer.Helpers.EventCommand;
using RayTracer.Model;

namespace RayTracer.ViewModel
{
    public class KeyboardEventManager
    {
        #region Private Members
        /// <summary>
        /// The instance of the KeyboardEventManager
        /// </summary>
        private static KeyboardEventManager _instance;
        /// <summary>
        /// How much should the cursor be moved in one step
        /// </summary>
        private const double MoveStep = 0.1;
        #endregion Private Members
        #region Public Properties
        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static KeyboardEventManager Instance { get { return _instance ?? (_instance = new KeyboardEventManager()); } }
        #endregion Public Properties
        #region Constructors
        #endregion Constructors
        #region Public Methods
        #endregion Public Methods
        #region Commands
        private ActionCommand<KeyEventArgs> _keyUpCommand;
        public ActionCommand<KeyEventArgs> KeyUpCommand
        {
            get
            {
                return _keyUpCommand ??
                       (_keyUpCommand = new ActionCommand<KeyEventArgs>(KeyUpExecuted));
            }
        }
        /// <summary>
        /// Moved the cursor up/forward
        /// </summary>
        private void KeyUpExecuted(KeyEventArgs args)
        {
            if (Keyboard.IsKeyDown(Key.Z))
                Cursor3D.Instance.ModelTransform = Transformations.TranslationMatrix(new Vector3D(0, 0, MoveStep)) * Cursor3D.Instance.ModelTransform;
            else
                Cursor3D.Instance.ModelTransform = Transformations.TranslationMatrix(new Vector3D(0, -MoveStep, 0)) * Cursor3D.Instance.ModelTransform;
        }

        private ActionCommand<KeyEventArgs> _keyDownCommand;
        public ActionCommand<KeyEventArgs> KeyDownCommand
        {
            get
            {
                return _keyDownCommand ??
                       (_keyDownCommand = new ActionCommand<KeyEventArgs>(KeyDownExecuted));
            }
        }
        /// <summary>
        /// Moves the cursor down/backwards
        /// </summary>
        private void KeyDownExecuted(KeyEventArgs args)
        {
            if (Keyboard.IsKeyDown(Key.Z))
                Cursor3D.Instance.ModelTransform = Transformations.TranslationMatrix(new Vector3D(0, 0, -MoveStep)) * Cursor3D.Instance.ModelTransform;
            else
                Cursor3D.Instance.ModelTransform = Transformations.TranslationMatrix(new Vector3D(0, MoveStep, 0)) * Cursor3D.Instance.ModelTransform;
        }

        private ActionCommand<KeyEventArgs> _keyLeftCommand;
        public ActionCommand<KeyEventArgs> KeyLeftCommand
        {
            get
            {
                return _keyLeftCommand ??
                       (_keyLeftCommand = new ActionCommand<KeyEventArgs>(KeyLeftExecuted));
            }
        }
        /// <summary>
        /// Moves the cursor left
        /// </summary>
        private void KeyLeftExecuted(KeyEventArgs args)
        {
            Cursor3D.Instance.ModelTransform = Transformations.TranslationMatrix(new Vector3D(-MoveStep, 0, 0)) * Cursor3D.Instance.ModelTransform;
        }

        private ActionCommand<KeyEventArgs> _keyRightCommand;
        public ActionCommand<KeyEventArgs> KeyRightCommand
        {
            get
            {
                return _keyRightCommand ??
                       (_keyRightCommand = new ActionCommand<KeyEventArgs>(KeyRightExecuted));
            }
        }
        /// <summary>
        /// Moves the cursor right
        /// </summary>
        private void KeyRightExecuted(KeyEventArgs args)
        {
            Cursor3D.Instance.ModelTransform = Transformations.TranslationMatrix(new Vector3D(MoveStep, 0, 0)) * Cursor3D.Instance.ModelTransform;
        }
        #endregion Commands
    }
}
