using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FluentDragDrop
{
	internal class BitmapPreview : IPreview
	{
		public event EventHandler<Preview> Updated;

		private readonly Preview _preview;

		public BitmapPreview(Bitmap bitmap)
		{
			if (bitmap == null)
				throw new ArgumentNullException(nameof(bitmap));

			_preview = new Preview(bitmap);
		}

		public Preview Get() => _preview;


		public void Start()
		{
		}

		public void Stop()
		{
		}
	}
}
