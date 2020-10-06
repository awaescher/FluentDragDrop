using System;
using System.Drawing;

namespace FluentDragDrop.Preview
{
    internal class BitmapPreview : IPreview
    {
		private Bitmap _bitmap;

		public BitmapPreview(Bitmap bitmap)
        {
            _bitmap = bitmap ?? throw new ArgumentNullException(nameof(bitmap));
        }

        public void Render(Graphics graphics)
        {
            if (_bitmap is object)
                graphics.DrawImageUnscaled(_bitmap, new Rectangle(Point.Empty, _bitmap.Size));
        }

        public Size PreferredSize => _bitmap?.Size ?? Size.Empty;

        /// <summary>
        /// Gets or sets the opacity of the preview during the drag and drop operation
        /// </summary>
        public double Opacity { get; set; } = 0.8;
	}
}