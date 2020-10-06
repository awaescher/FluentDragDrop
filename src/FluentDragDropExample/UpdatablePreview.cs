using FluentDragDrop.Preview;
using System;
using System.Drawing;
using System.Threading;

namespace FluentDragDropExample
{
	internal class UpdatablePreview : IUpdatablePreview
	{
		public event EventHandler Updated;

		private readonly Bitmap _originalImage;
		private Timer _timer;
		private readonly Point _mouseStartPosition;
		private double _distance;
		private Font _font;
		private StringFormat _format;

		public UpdatablePreview(Bitmap original, Point mouseStartPosition)
		{
			_originalImage = original ?? throw new ArgumentNullException(nameof(original));
			_mouseStartPosition = mouseStartPosition;

			UpdatePreview(null);
		}

		public void Start()
		{
			_font = new Font("Tahoma", 11);
			_format = new StringFormat
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Far
			};
			_timer = new Timer(UpdatePreview, null, TimeSpan.FromMilliseconds(20), TimeSpan.FromMilliseconds(20));
		}

		public void Stop()
		{
			_timer?.Dispose();
			_timer = null;

			_font?.Dispose();
			_format?.Dispose();
		}

		private void UpdatePreview(object state)
		{
			var currentMousePosition = System.Windows.Forms.Control.MousePosition;
			var distanceX = Math.Abs(currentMousePosition.X - _mouseStartPosition.X);
			var distanceY = Math.Abs(currentMousePosition.Y - _mouseStartPosition.Y);
			_distance = Math.Round(Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2)));

			// at 900 distance, we want it to be transparent
			Opacity = (900 - _distance) / 900;
			Updated?.Invoke(this, EventArgs.Empty);
		}

		public void Render(Graphics graphics)
		{
			var bounds = new Rectangle(0, 20, _originalImage.Width, _originalImage.Height - 40);
			graphics.DrawImageUnscaled(_originalImage, new Rectangle(Point.Empty, PreferredSize));
			graphics.DrawString($"Distance: {_distance}px", _font, Brushes.White, bounds, _format);
		}

		public Size PreferredSize => _originalImage?.Size ?? Size.Empty;

		public double Opacity { get; set; } = 0.8;
	}
}