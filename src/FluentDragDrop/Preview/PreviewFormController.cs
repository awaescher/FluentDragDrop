using System;
using System.Drawing;
using System.Windows.Forms;

namespace FluentDragDrop.Preview
{
    internal class PreviewFormController
    {
        [ThreadStatic]
        private static PreviewForm _previewForm;

        internal static PreviewForm PreviewForm => _previewForm ??= new PreviewForm();

        public PreviewFormController()
        {
            IsDragging = false;
        }

        public void Start(PreviewElement preview, Point cursorOffset)
        {
            if (preview is null)
                return;

            CursorOffset = cursorOffset;

            var mousePosition = Control.MousePosition;
            var previewPosition = new Point(mousePosition.X - cursorOffset.X, mousePosition.Y - cursorOffset.Y);

            if (_previewForm?.IsDisposed ?? false)
            {
                _previewForm = null;
            }

            Stop();

            IsDragging = true;

            PreviewForm.Start(previewPosition, preview);
        }

        internal void Update(PreviewElement preview)
        {
            if (preview is null)
            {
                return;
            }

            PreviewForm.Update(preview);
        }

        internal void Stop()
        {
            if (!IsDragging)
            {
                return;
            }

            IsDragging = false;
            PreviewForm.Stop();
        }

        public void Move()
        {
            var mousePosition = Control.MousePosition;
            var position = new Point(mousePosition.X - CursorOffset.X, mousePosition.Y - CursorOffset.Y);

            if (!IsDragging)
            {
                return;
            }

            PreviewForm.Move(position);
        }

        private bool IsDragging { get; set; }

        private Point CursorOffset { get; set; }
    }
}
