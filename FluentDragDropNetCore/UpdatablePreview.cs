using FluentDragDrop;
using System;
using System.Drawing;
using System.Threading;

namespace FluentDragDrop
{
	class UpdatablePreview : IPreview
	{
		public event EventHandler Updated;

		public Bitmap _original;

		private Bitmap _current;
		private Timer _timer;

		public UpdatablePreview(Bitmap original)
		{
			_original = original ?? throw new ArgumentNullException(nameof(original));
			_current = original;
		}

		private void UpdatePreview(object state)
		{
			_current = _original;

			using (var graphics = Graphics.FromImage(_current))
			{
				using (var font = new Font("Tahoma", 11))
				{
					using (var format = new StringFormat())
					{
						format.Alignment = StringAlignment.Center;
						format.LineAlignment = StringAlignment.Far;

						var bounds = new Rectangle(Point.Empty, _original.Size);
						graphics.DrawString(DateTime.Now.ToString(), font, Brushes.White, bounds, format);
					}
				}
			}

			Updated?.Invoke(this, EventArgs.Empty);
		}

		public Bitmap Get() => _current;

		public void Start()
		{
			_timer = new Timer(UpdatePreview, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
		}

		public void Stop()
		{
			_timer?.Dispose();
			_timer = null;
		}
	}
}
