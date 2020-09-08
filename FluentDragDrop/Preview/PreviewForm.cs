using System;
using System.Drawing;
using System.Windows.Forms;

namespace FluentDragDrop
{
	internal class PreviewForm : Form
	{
		private const int WM_CAPTURECHANGED = 0x215;
		private const uint WM_NCHITTEST = 0x84;
		private const int HTTRANSPARENT = -1;

		public PreviewForm()
		{
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

			Hide();

			IsDragging = false;
			MinimumSize = Size.Empty;
			Size = Size.Empty;
			StartPosition = FormStartPosition.Manual;
			Location = new Point(-9999, -9999);
			Enabled = false;
			TabStop = false;
			BackColor = Color.Transparent;
			TopMost = true;
			FormBorderStyle = FormBorderStyle.None;
		}

		public void Start(Point location, Preview preview)
		{
			if (IsDisposed)
				return;

			if (!IsHandleCreated)
				CreateHandle();

			IsDragging = true;
			Visible = true;

			Location = location;
			Update(preview);
		}

		public void Update(Preview preview)
		{
			if (IsDisposed)
				return;

			PreviewBitmap = preview?.Bitmap;

			if (PreviewBitmap == null)
			{
				Hide();
			}
			else
			{
				if (PreviewBitmap.Size != Size)
					Size = PreviewBitmap.Size;
			}

			Opacity = preview.Opacity;
			Invalidate();
		}

		public new void Move(Point location)
		{
			if (IsDragging)
				Location = location;
		}

		public void Stop()
		{
			if (IsDragging)
			{
				Hide();
				PreviewBitmap = null;
				IsDragging = false;
				Location = new Point(-9999, -9999);
			}
		}

		protected override void WndProc(ref Message m)
		{
			int msg = m.Msg;

			if (m.Msg == WM_NCHITTEST)
			{
				// make the window input transparent to not block the mose
				m.Result = new IntPtr(HTTRANSPARENT);
				return;
			}

			if (msg == WM_CAPTURECHANGED)
			{
				if (m.LParam != base.Handle)
					FormCaptureLost();
			}

			base.WndProc(ref m);
		}

		protected void FormCaptureLost()
		{
			Stop();
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			// DO NOT paint the background to prevent flickering 
			// when a Control is double buffered, painting in OnPaint() only helps to reduce flickering
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (PreviewBitmap != null)
				e.Graphics.DrawImageUnscaled(PreviewBitmap, new Rectangle(Point.Empty, Size));
		}

		private bool IsDragging { get; set; }

		private Bitmap PreviewBitmap { get; set; }
	}
}
