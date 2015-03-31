using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using RayTracer.Helpers;
using RayTracer.Helpers.EventCommand;
using RayTracer.Model;
using RayTracer.Model.Shapes;

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
        private const double Tolernce = 0.3;
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
            {
                foreach (var point in PointManager.Instance.SelectedItems)
                    point.ModelTransform = Transformations.TranslationMatrix(new Vector3D(0, 0, MoveStep)) * point.ModelTransform;
                Cursor3D.Instance.ModelTransform = Transformations.TranslationMatrix(new Vector3D(0, 0, MoveStep)) * Cursor3D.Instance.ModelTransform;
            }
            else
            {
                foreach (var point in PointManager.Instance.SelectedItems)
                    point.ModelTransform = Transformations.TranslationMatrix(new Vector3D(0, -MoveStep, 0)) * point.ModelTransform;
                Cursor3D.Instance.ModelTransform = Transformations.TranslationMatrix(new Vector3D(0, -MoveStep, 0)) * Cursor3D.Instance.ModelTransform;
            }
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
            {
                foreach (var point in PointManager.Instance.SelectedItems)
                    point.ModelTransform = Transformations.TranslationMatrix(new Vector3D(0, 0, -MoveStep)) * point.ModelTransform;
                Cursor3D.Instance.ModelTransform = Transformations.TranslationMatrix(new Vector3D(0, 0, -MoveStep)) * Cursor3D.Instance.ModelTransform;
            }
            else
            {
                foreach (var point in PointManager.Instance.SelectedItems)
                    point.ModelTransform = Transformations.TranslationMatrix(new Vector3D(0, MoveStep, 0)) * point.ModelTransform;
                Cursor3D.Instance.ModelTransform = Transformations.TranslationMatrix(new Vector3D(0, MoveStep, 0)) * Cursor3D.Instance.ModelTransform;
            }
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
            foreach (var point in PointManager.Instance.SelectedItems)
                point.ModelTransform = Transformations.TranslationMatrix(new Vector3D(-MoveStep, 0, 0)) * point.ModelTransform;
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
            foreach (var point in PointManager.Instance.SelectedItems)
                point.ModelTransform = Transformations.TranslationMatrix(new Vector3D(MoveStep, 0, 0)) * point.ModelTransform;
            Cursor3D.Instance.ModelTransform = Transformations.TranslationMatrix(new Vector3D(MoveStep, 0, 0)) * Cursor3D.Instance.ModelTransform;
        }

        private ActionCommand<KeyEventArgs> _keyDeleteCommand;
        public ActionCommand<KeyEventArgs> KeyDeleteCommand
        {
            get
            {
                return _keyDeleteCommand ??
                       (_keyDeleteCommand = new ActionCommand<KeyEventArgs>(KeyDeleteExecuted));
            }
        }
        /// <summary>
        /// Removes the selected point
        /// </summary>
        private void KeyDeleteExecuted(KeyEventArgs args)
        {
            foreach (var point in PointManager.Instance.SelectedItems)
                PointManager.Instance.Points.Remove(point);
        }

        private ActionCommand<KeyEventArgs> _keySelectCommand;
        public ActionCommand<KeyEventArgs> KeySelectCommand
        {
            get
            {
                return _keySelectCommand ??
                       (_keySelectCommand = new ActionCommand<KeyEventArgs>(KeySelectExecuted));
            }
        }

        /// <summary>
        /// Selects / deselects the point
        /// </summary>
        private void KeySelectExecuted(KeyEventArgs args)
        {
            var x = Cursor3D.Instance.XPosition;
            var y = Cursor3D.Instance.YPosition;
            var z = Cursor3D.Instance.ZPosition;

            var items = PointManager.Instance.SelectedItems;
            PointEx point = null;

            foreach (var p in PointManager.Instance.Points)
            {
                var transformedPoint = p.ModelTransform * new Vector4(p.X, p.Y, p.Z, 1);
                if (transformedPoint.X < x + Tolernce && transformedPoint.X > x - Tolernce
                    && transformedPoint.Y < y + Tolernce && transformedPoint.Y > y - Tolernce
                    && transformedPoint.Z < z + Tolernce && transformedPoint.Z > z - Tolernce)
                    point = p;
            }

            if (point == null) return;
            if (items.Contains(point))
                items.Remove(point);
            else
            {
                items.Clear();
                items.Add(point);
            }
        }

        private ActionCommand<KeyEventArgs> _keyInsertCommand;
        public ActionCommand<KeyEventArgs> KeyInsertCommand
        {
            get
            {
                return _keyInsertCommand ??
                       (_keyInsertCommand = new ActionCommand<KeyEventArgs>(KeyInsertExecuted));
            }
        }
        /// <summary>
        /// Adds a new point in the position of the 3D cursor
        /// </summary>
        private void KeyInsertExecuted(KeyEventArgs args)
        {
            var x = Cursor3D.Instance.XPosition;
            var y = Cursor3D.Instance.YPosition;
            var z = Cursor3D.Instance.ZPosition;

            var point = PointManager.Instance.Points.FirstOrDefault(
                p => p.X < x + Tolernce && p.X > x - Tolernce && p.Y < y + Tolernce && p.Y > y - Tolernce && p.Z < z + Tolernce && p.Z > z - Tolernce);

            if (point == null)
                PointManager.Instance.Points.Add(new PointEx(x, y, z));
        }
        #endregion Commands
    }
}
