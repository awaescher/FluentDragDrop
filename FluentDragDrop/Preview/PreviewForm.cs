using System;
using System.Drawing;
using System.Windows.Forms;

namespace FluentDragDrop
{
	internal class PreviewForm : Form
	{
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private extern static IntPtr SetActiveWindow(IntPtr handle);

		private const uint WM_NCHITTEST = 0x84;
		private const int WM_ACTIVATE = 6;
		private const int WM_CAPTURECHANGED = 0x215;
		private const int WM_MOUSEACTIVATE = 0x0021;

		private const int MA_NOACTIVATEANDEAT = 0x0004;

		private const int WA_INACTIVE = 0;

		private const int WS_EX_NOACTIVATE = 0x08000000;
		private const int WS_EX_TOPMOST = 0x00000008;

		private const int HTTRANSPARENT = -1;

		public PreviewForm()
		{
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.Selectable, false);

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

			Location = location;
			Update(preview);

			Show();
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

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams param = base.CreateParams;
				param.ExStyle |= WS_EX_TOPMOST; // make the form topmost
				param.ExStyle |= WS_EX_NOACTIVATE; // prevent the form from being activated
				return param;
			}
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_MOUSEACTIVATE)
			{
				// prevent the form from being clicked and gaining focus
				m.Result = (IntPtr)MA_NOACTIVATEANDEAT;
				return;
			}
			else if (m.Msg == WM_ACTIVATE)
			{
				// if a message gets through to activate the form somehow
				if (((int)m.WParam & 0xFFFF) != WA_INACTIVE)
				{
					if (m.LParam != IntPtr.Zero)
						SetActiveWindow(m.LParam);
					else
						SetActiveWindow(IntPtr.Zero); // Could not find sender, just in-activate it.
				}
			}
			else if (m.Msg == WM_NCHITTEST)
			{
				// make the window input transparent to not block the mose
				m.Result = new IntPtr(HTTRANSPARENT);
				return;
			}
			else if (m.Msg == WM_CAPTURECHANGED)
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
