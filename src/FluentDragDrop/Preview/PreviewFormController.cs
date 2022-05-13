using System;
using System.Drawing;
using System.Windows.Forms;

namespace FluentDragDrop.Preview
{
	internal class PreviewFormController
	{
		private PreviewForm _previewForm;

		internal PreviewForm PreviewForm => _previewForm ??= new PreviewForm();

		public PreviewFormController()
		{
			IsDragging = false;
		}

		public void Start(Control sourceControl, Effects.Effects effects, IPreview preview, Point cursorOffset)
		{
			if (preview is null)
				return;

			CursorOffset = cursorOffset;

			var mousePosition = Control.MousePosition;
			var previewPosition = new Point(mousePosition.X + cursorOffset.X, mousePosition.Y + cursorOffset.Y);

			if (_previewForm?.IsDisposed ?? false)
				_previewForm = null;

			IsDragging = true;

			PreviewForm.Start(sourceControl, effects, previewPosition, preview);
		}

		internal void InvalidatePreview()
		{
			PreviewForm.InvalidatePreview();
		}

		internal void Stop(Control target, bool wasCancelled)
		{
			if (!IsDragging)
				return;

			IsDragging = false;
			PreviewForm.Stop(target, wasCancelled);
		}

		public void Move()
		{
			var mousePosition = Control.MousePosition;
			var position = new Point(mousePosition.X + CursorOffset.X, mousePosition.Y + CursorOffset.Y);

			if (!IsDragging)
				return;

			PreviewForm.Move(position);
		}

		private bool IsDragging { get; set; }

		private Point CursorOffset { get; set; }
	}
}