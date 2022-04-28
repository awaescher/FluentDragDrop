using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;

namespace FluentDragDrop.Preview
{
	public class OverlayForm : Form
	{
		[DllImport("USER32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int cmdShow);

		[DllImport("USER32.dll")]
		internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		internal const int WS_EX_TOOLWINDOW = 128;
		internal const int MA_NOACTIVATE = 3;
		internal const int SWP_NOSIZE = 1;
		internal const int SWP_NOMOVE = 2;
		internal const int SWP_NOACTIVATE = 16;
		internal const int SWP_SHOWWINDOW = 64;
		internal const int HWND_TOP = 0;
		internal const int HWND_TOPMOST = -1;
		internal const int WM_GETMINMAXINFO = 0x24;
		internal const int WM_MOUSEACTIVATE = 0x0021;
		internal const int WM_LBUTTONDOWN = 0x0201;
		internal const int WM_CAPTURECHANGED = 0x215;
		internal const int WM_NCHITTEST = 0x84;
		internal const int HTTRANSPARENT = -1;

		public OverlayForm()
		{
			Parent = null;
			TopLevel = true;
			ControlBox = false;
			StartPosition = FormStartPosition.Manual;
			FormBorderStyle = FormBorderStyle.None;
			ShowInTaskbar = false;
			Visible = false;
			TabStop = false;

			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, true);
			BackColor = Color.Transparent;


		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				createParams.ExStyle |= WS_EX_TOOLWINDOW;
				return createParams;
			}
		}

		protected virtual bool IsTopMost => true;

		protected virtual IntPtr InsertAfterWindow => (IntPtr)(IsTopMost ? -1 : 0);

		protected override bool ShowWithoutActivation => true;

		protected virtual bool AllowMouseActivate => false;

		protected virtual bool IsHitTestTransparent => true;

		protected virtual void OnLostCapture()
		{
			// for overriding purposes
		}

		protected virtual void OnGotCapture()
		{
			// for overriding purposes
		}

		[SecuritySafeCritical]
		protected override void SetVisibleCore(bool value)
		{
			base.SetVisibleCore(value);

			if (!value || !IsHandleCreated)
				return;

			SetWindowPos(Handle, InsertAfterWindow, 0, 0, 0, 0, SWP_NOACTIVATE | SWP_SHOWWINDOW | SWP_NOSIZE | SWP_NOMOVE);
		}

		protected override void WndProc(ref Message m)
		{
			switch (m.Msg)
			{
				case WM_MOUSEACTIVATE:
					if (!AllowMouseActivate)
					{
						m.Result = (IntPtr)MA_NOACTIVATE;
						return;
					}
					break;
				case WM_NCHITTEST:
					if (IsHitTestTransparent)
					{
						m.Result = new IntPtr(HTTRANSPARENT);
						return;
					}
					break;
				case WM_LBUTTONDOWN:
					break;
				case WM_GETMINMAXINFO:
					OnGetMinMaxInfo(ref m);
					return;
				case WM_CAPTURECHANGED:
					if (m.LParam == this.Handle)
						OnGotCapture();
					else
						OnLostCapture();
					break;
			}
			base.WndProc(ref m);
		}

		[SecuritySafeCritical]
		private void OnGetMinMaxInfo(ref Message m)
		{
			base.WndProc(ref m);

			var from = (MINMAXINFO)Marshal.PtrToStructure(m.LParam, typeof(MINMAXINFO));
			from.ptMinTrackSize = new POINT(1, 1);
			Marshal.StructureToPtr<MINMAXINFO>(from, m.LParam, true);
			m.Result = IntPtr.Zero;
		}
	}
}

public struct MINMAXINFO
{
	public POINT ptReserved;
	public POINT ptMaxSize;
	public POINT ptMaxPosition;
	public POINT ptMinTrackSize;
	public POINT ptMaxTrackSize;
}

public struct POINT
{
	public POINT(int x, int y)
	{
		X = x;
		Y = y;
	}

	public POINT(Point pt)
	{
		X = pt.X;
		Y = pt.Y;
	}

	public Point ToPoint() => new Point(X, Y);

	public int X;
	public int Y;
}

