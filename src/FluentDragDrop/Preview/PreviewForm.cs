using FluentDragDrop.Effects;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace FluentDragDrop.Preview
{
	internal class PreviewForm : OverlayForm
	{
		private Effects.Effects _effects;

		public PreviewForm()
		{
			MinimumSize = Size.Empty;
			Size = Size.Empty;
			Location = new Point(-5000, -5000);
			Enabled = false;
		}

		public void Start(Control sourceControl, Effects.Effects effects, Point location, IPreview preview)
		{
			if (IsDisposed)
				return;

			if (!IsHandleCreated)
				CreateHandle();

			_effects = effects;

			SourceControl = sourceControl ?? throw new ArgumentNullException(nameof(sourceControl));

			IsDragging = true;
			Location = location;

			Preview = preview;
			UpdatablePreview = preview as IUpdatablePreview;
			PreviewOpacityController = preview as IPreviewOpacityController;

			if (UpdatablePreview is object)
			{
				UpdatablePreview.Updated += OnUpdatablePreviewUpdated;
				UpdatablePreview?.Start();
			}

			InvalidatePreview();

			_effects.StartEffect.Start(new IEffect.Arguments(this, SourceControl, null));
		}

		public void Stop(Control target, bool wasCancelled)
		{
			if (!IsDragging)
				return;

			if (UpdatablePreview is object)
			{
				UpdatablePreview.Stop();
				UpdatablePreview.Updated -= OnUpdatablePreviewUpdated;
			}

			if (wasCancelled)
				_effects.CancelEffect.Start(new IEffect.Arguments(this, SourceControl, target));
			else
				_effects.DropEffect.Start(new IEffect.Arguments(this, SourceControl, target));

			IsDragging = false;

		}

		public void InvalidatePreview()
		{
			if (IsDisposed)
				return;

			if (!AllowUpdates)
				return;

			if (Preview == null)
			{
				Hide();
			}
			else
			{
				var preferredSize = Preview.PreferredSize;
				if (preferredSize != Size)
					Size = preferredSize;
			}

			Opacity = PreviewOpacityController?.Opacity ?? 0.8;
			TransparencyKey = PreviewOpacityController?.TransparencyKey ?? Color.Empty;
			Invalidate();
		}

		public new void Move(Point location)
		{
			if (IsDragging)
				Location = location;
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			// DO NOT paint the background to prevent flickering
			// when a Control is double buffered, painting in OnPaint() only helps to reduce flickering
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (AllowUpdates)
				Preview?.Render(e.Graphics);
		}

		private void OnUpdatablePreviewUpdated(object sender, EventArgs eventArgs)
		{
			if (InvokeRequired)
				BeginInvoke((Action)InvalidatePreview);
			else
				InvalidatePreview();
		}


		protected override void OnLostCapture()
		{
			Stop(null, wasCancelled: true);
		}

		private bool IsDragging { get; set; }

		private Control SourceControl { get; set; }

		private IPreview Preview { get; set; }

		public IUpdatablePreview UpdatablePreview { get; private set; }

		public IPreviewOpacityController PreviewOpacityController { get; private set; }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AllowUpdates { get; set; } = true;
	}
}
