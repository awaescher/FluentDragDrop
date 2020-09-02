using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FluentDragDrop
{
	public class DragOperation<T>
	{
		private ImageListDrag _dragger;

		private Dictionary<object, DragHandler<T>> _targets = new Dictionary<object, DragHandler<T>>();

		private bool _customPreview = false;

		private Point _cursorOffset = Point.Empty;
		public DragOperation(Control control, T data)
		{
			SourceControl = control ?? throw new ArgumentNullException(nameof(control));
			Data = data;
		}

		public DragOperation<T> To<TControl>(IEnumerable<TControl> targets, Action<TControl, T> dragDrop) where TControl : Control
		{
			foreach (var target in targets)
				To(target, dragDrop);

			return this;
		}

		public DragOperation<T> To<TControl>(TControl target, Action<TControl, T> dragDrop) where TControl : Control
		{
			var handler = DragHandler<T>.Default;
			handler.DragDrop = (value, _) => dragDrop?.Invoke(target, value);

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
				handler.DragDrop?.Invoke(Data, e);
		}

		private void Target_DragLeave(object sender, EventArgs e)
		{
			if (_targets.TryGetValue(sender, out var handler))
				handler.DragLeave?.Invoke(Data);
		}

		public DragOperation<T> WithoutPreview()
		{
			PreviewImage = null;
			_customPreview = true;

			return this;
		}

		public DragOperationPreview<T> WithPreview(Bitmap image)
		{
			PreviewImage = image;
			_customPreview = true;

			return new DragOperationPreview<T>(this);
		}

		public DragOperationPreview<T> WithPreview(Func<Bitmap, Bitmap> previewMutator)
		{
			PreviewImage = previewMutator(GetControlPreview());
			_customPreview = true;

			return new DragOperationPreview<T>(this);
		}

		public DragOperationPreview<T> WithPreview()
		{
			// reset the custom flag to paint the preview on drag start
			_customPreview = false;

			return new DragOperationPreview<T>(this);
		}

		internal DragOperation<T> WithCursorOffset(int x, int y)
		{
			_cursorOffset.X = -1 * x;
			_cursorOffset.Y = -1 * y;

			return this;
		}

		private Bitmap GetControlPreview()
		{
			var preview = new Bitmap(SourceControl.Width, SourceControl.Height);
			SourceControl.DrawToBitmap(preview, new Rectangle(Point.Empty, SourceControl.Size));

			return preview;
		}

		internal Size CalculatePreviewSize()
		{
			return PreviewImage?.Size ?? SourceControl?.Size ?? Size.Empty;
		}

		public void Copy() => Start(DragDropEffects.Copy);

		public void Link() => Start(DragDropEffects.Link);

		public void Move() => Start(DragDropEffects.Move);

		public void Start(DragDropEffects allowedEffects)
		{
			_dragger = new ImageListDrag();

			if (!_customPreview)
				PreviewImage = GetControlPreview();

			_dragger.StartDrag(PreviewImage ?? new Bitmap(1, 1), _cursorOffset.X, _cursorOffset.Y);

			void feedbackHandler(object _, GiveFeedbackEventArgs __) => _dragger?.DragDrop();

			try
			{
				SourceControl.GiveFeedback += feedbackHandler;

				var data = Data == null ? (object)new NullPlaceholder() : Data;
				SourceControl.DoDragDrop(data, allowedEffects);
			}
			finally
			{
				SourceControl.GiveFeedback -= feedbackHandler;

				_dragger.CompleteDrag();

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

		public Control SourceControl { get; }

		public T Data { get; }

		public Bitmap PreviewImage { get; private set; }
	}

	public struct NullPlaceholder { }
}
