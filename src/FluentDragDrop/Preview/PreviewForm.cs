using System;
using System.Drawing;
using System.Windows.Forms;

namespace FluentDragDrop.Preview
{
    internal sealed class PreviewForm : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SetActiveWindow(IntPtr handle);

        private const int WM_NCHITTEST = 0x84;
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
            ShowInTaskbar = false;
            Location = new Point(-9999, -9999);
            Enabled = false;
            TabStop = false;
            BackColor = Color.Transparent;
            TopMost = true;
            FormBorderStyle = FormBorderStyle.None;
        }

        public void Start(Point location, PreviewElement preview)
        {
            if (IsDisposed)
            {
                return;
            }

            if (!IsHandleCreated)
            {
                CreateHandle();
            }

            IsDragging = true;

            Location = location;
            Update(preview);

            Show();
        }

        public void Update(PreviewElement preview)
        {
            if (IsDisposed)
            {
                return;
            }

            PreviewBitmap = preview?.Bitmap;

            if (PreviewBitmap == null)
            {
                Hide();
            }
            else if (PreviewBitmap.Size != Size)
            {
                Size = PreviewBitmap.Size;
            }

            Opacity = preview?.Opacity ?? 0.8;
            Invalidate();
        }

        public new void Move(Point location)
        {
            if (IsDragging)
            {
                Location = location;
            }
        }

        public void Stop()
        {
            if (!IsDragging)
            {
                return;
            }

            Hide();
            PreviewBitmap = null;
            IsDragging = false;
            Location = new Point(-9999, -9999);
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
            switch (m.Msg)
            {
                case WM_MOUSEACTIVATE:
                    // prevent the form from being clicked and gaining focus
                    m.Result = (IntPtr)MA_NOACTIVATEANDEAT;
                    return;
                case WM_ACTIVATE:
                    // if a message gets through to activate the form somehow
                    if (((int)m.WParam & 0xFFFF) != WA_INACTIVE)
                    {
                        if (m.LParam != IntPtr.Zero)
                        {
                            SetActiveWindow(m.LParam);
                        }
                        else
                        {
                            // Could not find sender, just in-activate it.
                            SetActiveWindow(IntPtr.Zero);
                        }
                    }
                    break;
                case WM_NCHITTEST:
                    m.Result = new IntPtr(HTTRANSPARENT);
                    return;
                case WM_CAPTURECHANGED:
                    if (m.LParam != base.Handle)
                    {
                        FormCaptureLost();
                    }

                    break;
            }

            base.WndProc(ref m);
        }

        private void FormCaptureLost()
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
            if (PreviewBitmap == null)
            {
                return;
            }
            e.Graphics.DrawImageUnscaled(PreviewBitmap, new Rectangle(Point.Empty, Size));
        }

        private bool IsDragging { get; set; }

        private Bitmap PreviewBitmap { get; set; }
    }
}
