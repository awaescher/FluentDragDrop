using System.Drawing;

namespace FluentDragDrop
{
	public class Preview
	{
		public Preview(Bitmap bitmap)
		{
			Bitmap = bitmap;
		}

		public Preview(Bitmap bitmap, double opacity)
		{
			Bitmap = bitmap;
			Opacity = opacity;
		}

		public Bitmap Bitmap { get; set; }

		public double Opacity { get; set; } = 0.8;
	}
}
