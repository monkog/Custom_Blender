using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Media3D;
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
                foreach (var model in SceneManager.Instance.Models)
                    foreach (var point in model.SelectedItems)
                        point.ModelTransform = Transformations.TranslationMatrix(new Vector3D(0, 0, MoveStep)) * point.ModelTransform;
                Cursor3D.Instance.ModelTransform = Transformations.TranslationMatrix(new Vector3D(0, 0, MoveStep)) * Cursor3D.Instance.ModelTransform;
            }
            else
            {
                foreach (var point in PointManager.Instance.SelectedItems)
                    point.ModelTransform = Transformations.TranslationMatrix(new Vector3D(0, -MoveStep, 0)) * point.ModelTransform;
                foreach (var model in SceneManager.Instance.Models)
                    foreach (var point in model.SelectedItems)
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
                foreach (var model in SceneManager.Instance.Models)
                    foreach (var point in model.SelectedItems)
                        point.ModelTransform = Transformations.TranslationMatrix(new Vector3D(0, 0, -MoveStep)) * point.ModelTransform;
                Cursor3D.Instance.ModelTransform = Transformations.TranslationMatrix(new Vector3D(0, 0, -MoveStep)) * Cursor3D.Instance.ModelTransform;
            }
            else
            {
                foreach (var point in PointManager.Instance.SelectedItems)
                    point.ModelTransform = Transformations.TranslationMatrix(new Vector3D(0, MoveStep, 0)) * point.ModelTransform;
                foreach (var model in SceneManager.Instance.Models)
                    foreach (var point in model.SelectedItems)
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
            foreach (var model in SceneManager.Instance.Models)
                foreach (var point in model.SelectedItems)
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
            foreach (var model in SceneManager.Instance.Models)
                foreach (var point in model.SelectedItems)
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
            for (int i = CurveManager.Instance.SelectedItems.Count() - 1; i >= 0; i--)
                CurveManager.Instance.Curves.Remove(CurveManager.Instance.SelectedItems.ElementAt(i));
            for (int i = PointManager.Instance.SelectedItems.Count() - 1; i >= 0; i--)
            {
                var point = PointManager.Instance.SelectedItems.ElementAt(i);
                if (!point.CanBeDeleted) continue;
                foreach (var curve in CurveManager.Instance.Curves)
                {
                    if (curve.Continuity == Continuity.C0)
                        curve.Vertices.Remove(point);
                    else
                    {
                        if (((BezierCurveC2)curve).IsInterpolation)
                            ((BezierCurveC2)curve).InterpolationPoints.Remove(point);
                        else
                            ((BezierCurveC2)curve).DeBooreVertices.Remove(point);
                        ((BezierCurveC2)curve).UpdateVertices();
                    }
                }
                PointManager.Instance.Points.Remove(point);
            }
            for (int index = CurveManager.Instance.Curves.Count - 1; index >= 0; index--)
            {
                var curve = CurveManager.Instance.Curves[index];
                for (int i = curve.SelectedItems.Count() - 1; i >= 0; i--)
                    if (curve.Continuity == Continuity.C0)
                        curve.Vertices.Remove((PointEx)curve.SelectedItems.ElementAt(i));
                    else
                    {
                        if (((BezierCurveC2)curve).IsInterpolation)
                            ((BezierCurveC2)curve).InterpolationPoints.Remove((PointEx)curve.SelectedItems.ElementAt(i));
                        else
                            ((BezierCurveC2)curve).DeBooreVertices.Remove((PointEx)curve.SelectedItems.ElementAt(i));
                        ((BezierCurveC2)curve).UpdateVertices();
                    }
                if (curve.Vertices.Count == 0)
                    CurveManager.Instance.Curves.Remove(curve);
            }
            for (int i = PatchManager.Instance.SelectedItems.Count() - 1; i >= 0; i--)
                PatchManager.Instance.Patches.Remove(PatchManager.Instance.SelectedItems.ElementAt(i));
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
            var cursorTransform = Cursor3D.Instance.ModelTransform;

            var items = PointManager.Instance.SelectedItems;
            PointEx point = null;

            foreach (var p in PointManager.Instance.Points)
            {
                var transformedPoint = p.ModelTransform * p.Vector4;
                if (transformedPoint.X < x + Tolernce && transformedPoint.X > x - Tolernce
                    && transformedPoint.Y < y + Tolernce && transformedPoint.Y > y - Tolernce
                    && transformedPoint.Z < z + Tolernce && transformedPoint.Z > z - Tolernce)
                    point = p;
            }
            if (point == null)
            {
                foreach (var curve in CurveManager.Instance.Curves)
                    foreach (var p in curve.Vertices)
                    {
                        var transformedPoint = p.ModelTransform * p.Vector4;
                        if (transformedPoint.X < x + Tolernce && transformedPoint.X > x - Tolernce
                            && transformedPoint.Y < y + Tolernce && transformedPoint.Y > y - Tolernce
                            && transformedPoint.Z < z + Tolernce && transformedPoint.Z > z - Tolernce)
                            point = p;
                        else
                            p.IsSelected = false;
                    }

                foreach (var patch in PatchManager.Instance.Patches)
                    foreach (var p in patch.Vertices)
                    {
                        var transformedPoint = patch.ModelTransform * p.ModelTransform * p.Vector4;
                        if (transformedPoint.X < x + Tolernce && transformedPoint.X > x - Tolernce
                            && transformedPoint.Y < y + Tolernce && transformedPoint.Y > y - Tolernce)
                            p.IsSelected = !p.IsSelected;
                        else
                            p.IsSelected = false;
                    }
            }

            bool shouldSelect = point != null && !point.IsSelected;

            foreach (var p in items)
                p.IsSelected = false;

            if (shouldSelect)
                point.IsSelected = true;
            Cursor3D.Instance.ModelTransform = cursorTransform;
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

            var newPoint = new PointEx(x, y, z);
            foreach (var curve in CurveManager.Instance.SelectedItems)
                if (curve.Continuity == Continuity.C0)
                    curve.Vertices.Add(newPoint);
                else
                {
                    if (((BezierCurveC2)curve).IsInterpolation)
                        ((BezierCurveC2)curve).InterpolationPoints.Add(newPoint);
                    else
                        ((BezierCurveC2)curve).DeBooreVertices.Add(newPoint);
                    ((BezierCurveC2)curve).UpdateVertices();
                }
            PointManager.Instance.Points.Add(newPoint);
        }
        #endregion Commands
    }
}
