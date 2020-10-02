using FluentDragDrop.Preview;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FluentDragDrop
{
    public class DragOperation<T>
    {
        private PreviewFormController _previewController;

        private Dictionary<object, DragHandler<T>> _targets = new Dictionary<object, DragHandler<T>>();

        private Point _cursorOffset = Point.Empty;
        private Point _initialPosition = Point.Empty;
        private T _data;

        public DragOperation(DragDefinition definition, Func<T> dataEvaluator, Func<bool> conditionEvaluator)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            DataEvaluator = dataEvaluator ?? throw new ArgumentNullException(nameof(dataEvaluator));
            ConditionEvaluator = conditionEvaluator ?? throw new ArgumentNullException(nameof(conditionEvaluator));

            SourceControl = Definition.Control;

            WithPreview();

            StartOrTrackMouseToStart();
        }

        public DragOperation<T> To<TControl>(IEnumerable<TControl> targets, Action<TControl, T> dragDrop) where TControl : Control
        {
            foreach (var target in targets)
                To(target, dragDrop);

            return this;
        }

        public DragOperation<T> To<TControl>(TControl target, Action<TControl, T> dragDrop) where TControl : Control
        {
            var handler = DragHandler<T>.CreateDefault();
            handler.DragDrop = (value, _) =>
            {
                _previewController?.Stop();
                dragDrop?.Invoke(target, value);
            };

            return To(target, handler);
        }

        public DragOperation<T> To(Control target, DragHandler<T> handler)
        {
            target.AllowDrop = true;

            _targets[target] = handler;

            target.DragEnter += Target_DragEnter;
            target.DragOver += Target_DragOver;
            target.DragDrop += Target_DragDrop;
            target.DragLeave += Target_DragLeave;

            return this;
        }

        public DragOperation<T> WithoutPreview()
        {
            PreviewEvaluator = null;

            return this;
        }

        public DragOperationPreview<T> WithPreview(Bitmap image)
        {
            PreviewEvaluator = () => new BitmapPreview(image);

            return new DragOperationPreview<T>(this);
        }

        public DragOperationPreview<T> WithPreview(Func<Bitmap, T, Bitmap> previewMutator)
        {
            PreviewEvaluator = () => new BitmapPreview(previewMutator(GetControlPreviewBitmap(), Data));

            return new DragOperationPreview<T>(this);
        }

        public DragOperationPreview<T> WithPreview(IPreview preview)
        {
            PreviewEvaluator = () => preview;

            return new DragOperationPreview<T>(this);
        }

        public DragOperationPreview<T> WithPreview(Func<Bitmap, T, IPreview> previewMutator)
        {
            PreviewEvaluator = () => previewMutator(GetControlPreviewBitmap(), Data);

            return new DragOperationPreview<T>(this);
        }

        public DragOperationPreview<T> WithPreview()
        {
            PreviewEvaluator = () => new BitmapPreview(GetControlPreviewBitmap());

            return new DragOperationPreview<T>(this);
        }

        internal DragOperation<T> WithCursorOffset(int x, int y)
        {
            _cursorOffset.X = -1 * x;
            _cursorOffset.Y = -1 * y;

            return this;
        }

        private Bitmap GetControlPreviewBitmap()
        {
            var preview = new Bitmap(SourceControl.Width, SourceControl.Height);
            SourceControl.DrawToBitmap(preview, new Rectangle(Point.Empty, SourceControl.Size));

            return preview;
        }

        internal Size CalculatePreviewSize()
        {
            return PreviewEvaluator?.Invoke()?.Get()?.Bitmap?.Size ?? SourceControl?.Size ?? Size.Empty;
        }

        private void StartOrTrackMouseToStart()
        {
            if (Definition is ImmediateDragDefinition)
            {
                // enqueue in UI thread to make sure the whole fluent setup has been executed before.
                SourceControl.BeginInvoke((Action)(() => Start(Definition.AllowedEffects)));
            }
            else
            {
                _initialPosition = Control.MousePosition;
                SourceControl.MouseMove += SourceControl_MouseMove;
                SourceControl.MouseUp += SourceControl_MouseUp; // TODO only mouse up might be too soft. Esc? MouseLeave?
            }
        }

        private void Start(DragDropEffects allowedEffects)
        {
            StopTrackingForDeferredStartOnMouseMove();

            var canProceed = ConditionEvaluator.Invoke();
            if (!canProceed)
                return;

            _previewController = new PreviewFormController();

            var preview = PreviewEvaluator?.Invoke();

            preview?.Start();

            void updatePreview(object _, Preview.Preview updatedPreview)
            {
                if (SourceControl.InvokeRequired)
                    SourceControl.BeginInvoke((Action)(() => _previewController.Update(updatedPreview)));
                else
                    _previewController.Update(updatedPreview);
            }

            var hookId = IntPtr.Zero;

            try
            {
                if (preview is object)
                    preview.Updated += updatePreview;

                hookId = NativeMethods.HookMouseMove(() => _previewController.Move());

                _previewController.Start(preview?.Get(), _cursorOffset);

                var data = Data == null ? (object)new NullPlaceholder() : Data;
                SourceControl.DoDragDrop(data, allowedEffects);
            }
            finally
            {
                NativeMethods.RemoveHook(hookId);

                if (preview is object)
                    preview.Updated -= updatePreview;

                _previewController.Stop();
                preview?.Stop();

                CleanUp();
            }
        }

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
                _previewController?.Stop();
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

        private void StopTrackingForDeferredStartOnMouseMove()
        {
            _initialPosition = Point.Empty;
            SourceControl.MouseMove -= SourceControl_MouseMove;
            SourceControl.MouseUp -= SourceControl_MouseUp;
        }

        private void SourceControl_MouseMove(object sender, MouseEventArgs e)
        {
            const int DISTANCE = 3;

            var currentPosition = Control.MousePosition;
            var deltaX = Math.Abs(currentPosition.X - _initialPosition.X);
            var deltaY = Math.Abs(currentPosition.Y - _initialPosition.Y);

            if (deltaX > DISTANCE || deltaY > DISTANCE)
                Start(Definition.AllowedEffects);
        }

        public Control SourceControl { get; }

        public T Data
        {
            get
            {
                if (_data is null)
                    _data = DataEvaluator.Invoke();

                return _data;
            }
        }

        public DragDefinition Definition { get; }

        public Func<T> DataEvaluator { get; }

        public Func<bool> ConditionEvaluator { get; }

        public Func<IPreview> PreviewEvaluator { get; private set; }
    }

    public struct NullPlaceholder { }
}
