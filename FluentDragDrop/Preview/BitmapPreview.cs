using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FluentDragDrop
{
	internal class BitmapPreview : IPreview
	{
		public event EventHandler Updated;

		private readonly Bitmap _preview;

		public BitmapPreview(Bitmap preview)
		{
			_preview = preview ?? throw new ArgumentNullException(nameof(preview));
		}

		public Bitmap Get() => _preview;


		public void Start()
		{
		}

		public void Stop()
		{
		}
	}
}
