using System;
using System.Drawing;

namespace FluentDragDrop.Preview
{
    internal class BitmapPreview : IPreview
    {
		private readonly Func<Bitmap> _evaluator;
		private Bitmap _bitmap;

		public BitmapPreview(Func<Bitmap> evaluator)
        {
            _evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
        }

        public void Render(Graphics graphics)
        {
            if (PreviewBitmap is Bitmap bmp)
                graphics.DrawImageUnscaled(bmp, new Rectangle(Point.Empty, _bitmap.Size));
        }

        public Size PreferredSize => PreviewBitmap?.Size ?? Size.Empty;

		private Bitmap PreviewBitmap => _bitmap ??= _evaluator.Invoke();
	}
}