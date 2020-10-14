using FluentDragDrop.Preview;
using FluentTransitions;
using System;
using System.Drawing;
using System.Threading;

namespace FluentDragDropExample
{
	internal class ExperimentalPhysics : IUpdatablePreview, IPreviewOpacityController
	{
		public event EventHandler Updated;

		private readonly Bitmap _originalImage;
		private Timer _timer;
		private Point _lastMousePosition;
		private Point _distance;
		private Font _font;
		private StringFormat _format;
		private float _angle;

		public ExperimentalPhysics(Bitmap original, Point mouseStartPosition)
		{
			_originalImage = original ?? throw new ArgumentNullException(nameof(original));
			_lastMousePosition = mouseStartPosition;

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

			Transition
				.With(this, nameof(Angle), 45f)
				.EaseInEaseOut(TimeSpan.FromMilliseconds(300));
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
			var distanceX = currentMousePosition.X - _lastMousePosition.X;
			var distanceY = currentMousePosition.Y - _lastMousePosition.Y;
			_distance = new Point(distanceX, distanceY);

			_lastMousePosition = currentMousePosition;

			//_transition = new Transition(new TransitionType_EaseInEaseOut(100));
			//var maxDistance = Math.Max(-90, Math.Min(90, _distance.X));
			//System.Diagnostics.Debug.WriteLine(maxDistance);
			//System.Diagnostics.Debug.WriteLine((float)(45 + (float)maxDistance / 3));
			//_transition.add(this, nameof(Angle), (float)(45 + (float)maxDistance / 3));
			//_transition.run();
		}

		public float Angle
		{
			get => _angle;
			set 
			{
				if (_angle != value)
				{
					_angle = value;
					Updated?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		public void Render(Graphics graphics)
		{
			graphics.Clear(TransparencyKey);
			
			var maxDistance = Math.Max(-90, Math.Min(90, _distance.X));
			graphics.TranslateTransform(_originalImage.Width, 0);
			//graphics.RotateTransform(45 + (float)maxDistance / 3);
			graphics.RotateTransform(_angle);
			var bounds = new Rectangle(Point.Empty, PreferredSize);
			graphics.DrawImageUnscaled(_originalImage, bounds);
			graphics.ResetTransform();
		}

		public Size PreferredSize
		{
			get 
			{
				if (_originalImage is null)
					return Size.Empty;

				return new Size(_originalImage.Width * 2, _originalImage.Height * 2);
			}
		}

		public double Opacity { get; set; } = 0.8;

		public Color TransparencyKey => Color.Fuchsia;
	}
}