using FluentDragDrop.Effects;
using FluentDragDrop.Preview;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FluentDragDrop
{
	/// <summary>
	/// Coordinates the drag and drop operation
	/// </summary>
	/// <typeparam name="T">The type of the data to drop</typeparam>
	public class DragOperation<T>
    {
        private readonly Dictionary<object, DragHandler<T>> _targets = new Dictionary<object, DragHandler<T>>();

        private PreviewFormController _previewFormController;
        private readonly DragDefinition _definition;
        private readonly Func<T> _dataEvaluator;
        private readonly Func<bool> _conditionEvaluator;
        private Func<IPreview> _previewEvaluator;
        private T _data;
		private Effects.Effects _effects = Effects.Effects.GetDefaults();
		private bool _allowMouseHooks = true;

		private Point _cursorOffset = Point.Empty;
        private Point _initialPosition = Point.Empty;

        /// <summary>
        /// Creates a new instance of the drag operation
        /// </summary>
        /// <param name="definition">The drag definition containing the source control and the drag and drop effect</param>
        /// <param name="dataEvaluator">A function to retrieve the data to drop</param>
        /// <param name="conditionEvaluator">A function to check whether the operation should be started or not</param>
        public DragOperation(DragDefinition definition, Func<T> dataEvaluator, Func<bool> conditionEvaluator)
        {
            _definition = definition ?? throw new ArgumentNullException(nameof(definition));
            _dataEvaluator = dataEvaluator ?? throw new ArgumentNullException(nameof(dataEvaluator));
            _conditionEvaluator = conditionEvaluator ?? throw new ArgumentNullException(nameof(conditionEvaluator));

            SourceControl = _definition.Control;

            WithPreview();

            StartOrTrackMouseToStart();
        }

        /// <summary>
        /// Defines one or more target controls for the drag and drop operation.
        /// </summary>
        /// <typeparam name="TControl">The type of the control to drop on</typeparam>
        /// <param name="targets">The target controls to drop on</param>
        /// <param name="dragDrop">A function to handle the drop on one of the target controls</param>
        /// <returns></returns>
        public DragOperation<T> To<TControl>(IEnumerable<TControl> targets, Action<TControl, T> dragDrop) where TControl : Control
        {
            foreach (var target in targets)
                To(target, dragDrop);

            return this;
        }

        /// <summary>
        /// Defines a target control for the drag and drop operation.
        /// </summary>
        /// <typeparam name="TControl">The type of the control to drop on</typeparam>
        /// <param name="target">The target control to drop on</param>
        /// <param name="dragDrop">A function to handle the drop on the target control</param>
        /// <returns></returns>
        public DragOperation<T> To<TControl>(TControl target, Action<TControl, T> dragDrop) where TControl : Control
        {
            var handler = DragHandler<T>.CreateDefault();
            handler.DragDrop = (value, _) =>
            {
                _previewFormController?.Stop(target, wasCancelled: false);
                dragDrop?.Invoke(target, value);
            };

            return To(target, handler);
        }

        /// <summary>
        /// Attaches the required drag and drop events to a given target control.
        /// </summary>
        /// <param name="target">The target control to attach the events to</param>
        /// <param name="handler">A handle class to link to the target control</param>
        /// <returns></returns>
        private DragOperation<T> To(Control target, DragHandler<T> handler)
        {
            target.AllowDrop = true;

            _targets[target] = handler;

            target.DragEnter += Target_DragEnter;
            target.DragOver += Target_DragOver;
            target.DragDrop += Target_DragDrop;
            target.DragLeave += Target_DragLeave;

            return this;
        }

        /// <summary>
        /// Prevents the drag and drop operation from showing a preview image.
        /// </summary>
        /// <returns></returns>
        public DragOperation<T> WithoutPreview()
        {
            _previewEvaluator = null;

            return this;
        }

        /// <summary>
        /// Defines the preview image for the drag and drop operation
        /// </summary>
        /// <param name="image">The preview image for the drag and drop operation</param>
        /// <returns></returns>
        public DragOperationPreview<T> WithPreview(Bitmap image)
        {
            _previewEvaluator = () => new BitmapPreview(image);

            return new DragOperationPreview<T>(this);
        }

        /// <summary>
        /// Defines how the default preview image should be modified for the drag and drop operation
        /// </summary>
        /// <param name="previewMutator">A mutator function to modify the preview image for the drag and drop operation</param>
        /// <returns></returns>
        public DragOperationPreview<T> WithPreview(Func<Bitmap, T, Bitmap> previewMutator)
        {
            _previewEvaluator = () => new BitmapPreview(previewMutator(GetControlPreviewBitmap(), Data));

            return new DragOperationPreview<T>(this);
        }

        /// <summary>
        /// Defines the preview for the drag and drop operation
        /// </summary>
        /// <param name="preview">The preview implementation to show during the drag and drop operation</param>
        /// <returns></returns>
        public DragOperationPreview<T> WithPreview(IPreview preview)
        {
            _previewEvaluator = () => preview;

            return new DragOperationPreview<T>(this);
        }

        /// <summary>
        /// Defines how the default preview image should be modified for the drag and drop operation
        /// </summary>
        /// <param name="previewMutator">A mutator function to modify the preview for the drag and drop operation</param>
        /// <returns></returns>
        public DragOperationPreview<T> WithPreview(Func<Bitmap, T, IPreview> previewMutator)
        {
            _previewEvaluator = () => previewMutator(GetControlPreviewBitmap(), Data);

            return new DragOperationPreview<T>(this);
        }

        /// <summary>
        /// Defines the preview image to be taken from the control directly.
        /// </summary>
        /// <returns></returns>
        public DragOperationPreview<T> WithPreview()
        {
            _previewEvaluator = () => new BitmapPreview(GetControlPreviewBitmap());

            return new DragOperationPreview<T>(this);
        }

		/// <summary>
		/// Defines one or more effects to start when the drag and drop operation gets started.
		/// </summary>
		/// <param name="effects">The effects to start</param>
		/// <returns></returns>
		public DragOperation<T> WithStartEffects(params IEffect[] effects)
		{
			_effects.StartEffect = effects.Length == 1 ? effects[0] : new CompositeEffect(effects);
			return this;
		}

		/// <summary>
		/// Defines one or more effects to start when the drag and drop operation gets completed successfully.
		/// </summary>
		/// <param name="effects">The effects to start</param>
		/// <returns></returns>
		public DragOperation<T> WithDropEffects(params IEffect[] effects)
		{
			_effects.DropEffect = effects.Length == 1 ? effects[0] : new CompositeEffect(effects);
			return this;
		}

		/// <summary>
		/// Defines one or more effects to start when the draf and drop operation gets cancelled.
		/// </summary>
		/// <param name="effects">The effects to start</param>
		/// <returns></returns>
		public DragOperation<T> WithCancelEffects(params IEffect[] effects)
		{
			_effects.CancelEffect = effects.Length == 1 ? effects[0] : new CompositeEffect(effects);
			return this;
		}

		/// <summary>
		/// Defines the cursor offset of the preview image
		/// </summary>
		/// <param name="x">The offset on the X axis in pixels</param>
		/// <param name="y">The offset on the Y axis in pixels</param>
		/// <returns></returns>
		internal DragOperation<T> WithCursorOffset(int x, int y)
        {
            _cursorOffset.X = -1 * x;
            _cursorOffset.Y = -1 * y;

            return this;
        }

		/// <summary>
		/// Prevents mouse hooks and instead uses the Control.GiveFeedback event to update the preview image.
		/// This will have negative effects on the smoothness of the drag and drop operation.
		/// </summary>
		/// <returns></returns>
		public DragOperation<T> WithoutMouseHooks()
		{
			_allowMouseHooks = false;

			return this;
		}

		/// <summary>
		/// Gets a preview image from the source control
		/// </summary>
		/// <returns></returns>
		private Bitmap GetControlPreviewBitmap()
        {
            var preview = new Bitmap(SourceControl.Width, SourceControl.Height);
            SourceControl.DrawToBitmap(preview, new Rectangle(Point.Empty, SourceControl.Size));

            return preview;
        }

        /// <summary>
        /// Calculates the best size for the preview
        /// </summary>
        /// <returns></returns>
        internal Size CalculatePreviewSize()
        {
            return _previewEvaluator?.Invoke()?.PreferredSize ?? SourceControl?.Size ?? Size.Empty;
        }

        /// <summary>
        /// Begins to track the mouse to detect mouse movement to start deferred drag and drop operations
        /// </summary>
        private void StartOrTrackMouseToStart()
        {
            if (_definition is ImmediateDragDefinition)
            {
                // enqueue in UI thread to make sure the whole fluent setup has been executed before.
                SourceControl.BeginInvoke((Action)(() => Start(_definition.Effect)));
            }
            else
            {
                _initialPosition = Control.MousePosition;
                SourceControl.MouseMove += SourceControl_MouseMove;
                SourceControl.MouseUp += SourceControl_MouseUp; // TODO only mouse up might be too soft. Esc? MouseLeave?
            }
        }

        /// <summary>
        /// Starts the drag and drop operation
        /// </summary>
        /// <param name="effect">The desired drag and drop effect like Copy, Move or Link</param>
        private void Start(DragDropEffects effect)
        {
            StopTrackingForDeferredStartOnMouseMove();

            var canProceed = _conditionEvaluator.Invoke();
            if (!canProceed)
                return;

            _previewFormController = new PreviewFormController();

            var preview = _previewEvaluator?.Invoke();

            var hookId = IntPtr.Zero;

			var resultEffect = DragDropEffects.None;

            try
            {
				if (_allowMouseHooks)
					hookId = NativeMethods.HookMouseMove(() => _previewFormController.Move());
				else
					SourceControl.GiveFeedback += SourceControl_GiveFeedback;

                _previewFormController.Start(SourceControl, _effects, preview, _cursorOffset);

                var data = Data == null ? (object)new NullPlaceholder() : Data;
				resultEffect = SourceControl.DoDragDrop(data, effect);
            }
            finally
            {
				if (_allowMouseHooks)
					NativeMethods.RemoveHook(hookId);
				else
					SourceControl.GiveFeedback -= SourceControl_GiveFeedback;

				// always send "canceled" here -> if the drag and drop operation was successful
				// this call is too late and will not do anything
				_previewFormController.Stop(null, wasCancelled: true);

                CleanUp();
            }
        }

		private void SourceControl_GiveFeedback(object sender, GiveFeedbackEventArgs e)
		{
			_previewFormController.Move();
		}

		/// <summary>
		/// Removes the internally attached event handlers from all target controls
		/// </summary>
		private void CleanUp()
        {
            foreach (var target in _targets.Keys.OfType<Control>().ToArray())
            {
                target.DragEnter -= Target_DragEnter;
                target.DragOver -= Target_DragOver;
                target.DragDrop -= Target_DragDrop;
                target.DragLeave -= Target_DragLeave;
            }

            _targets.Clear();
        }

        /// <summary>
        /// Removes the internally attached event handlers to detect mouse movement
        /// for deferred drag and drop operations
        /// </summary>
        private void StopTrackingForDeferredStartOnMouseMove()
        {
            _initialPosition = Point.Empty;
            SourceControl.MouseMove -= SourceControl_MouseMove;
            SourceControl.MouseUp -= SourceControl_MouseUp;
        }

        private void Target_DragEnter(object sender, DragEventArgs e)
        {
            if (_targets.TryGetValue(sender, out var handler))
                handler.DragEnter?.Invoke(Data, e);
        }

        private void Target_DragOver(object sender, DragEventArgs e)
        {
            if (_targets.TryGetValue(sender, out var handler))
                handler.DragOver?.Invoke(Data, e);
        }

        private void Target_DragDrop(object sender, DragEventArgs e)
        {
            if (_targets.TryGetValue(sender, out var handler))
            {
                _previewFormController?.Stop(sender as Control, wasCancelled: false);
                handler.DragDrop?.Invoke(Data, e);
            }
        }

        private void Target_DragLeave(object sender, EventArgs e)
        {
            if (_targets.TryGetValue(sender, out var handler))
                handler.DragLeave?.Invoke(Data);
        }

        private void SourceControl_MouseUp(object sender, MouseEventArgs e)
        {
            StopTrackingForDeferredStartOnMouseMove();

            // I hunted this bug quite a long time: if the deferred drag operation is used,
            // the events are still attached in To() to a given control. So even if we do 
            // not start the drag-drop operation (with a click for example), we have to clean
            // up the event handlers or otherwise we'll have multiple handlers with old data
            CleanUp();
        }
        
        private void SourceControl_MouseMove(object sender, MouseEventArgs e)
        {
            const int DISTANCE = 3;

            var currentPosition = Control.MousePosition;
            var deltaX = Math.Abs(currentPosition.X - _initialPosition.X);
            var deltaY = Math.Abs(currentPosition.Y - _initialPosition.Y);

            if (deltaX > DISTANCE || deltaY > DISTANCE)
                Start(_definition.Effect);
        }

        /// <summary>
        /// The control which starts the drag and drop operation
        /// </summary>
        internal Control SourceControl { get; }

        /// <summary>
        /// The data to be passed through the drag and drop operation
        /// </summary>
        private T Data
        {
            get
            {
                if (_data is null)
                    _data = _dataEvaluator.Invoke();

                return _data;
            }
        }
    }
}