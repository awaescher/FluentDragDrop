
/* Unmerged change from project 'FluentDragDropExample (net45)'
Before:
using System;
using System.Drawing;
using System.Threading;
using FluentDragDrop;
using FluentDragDrop.Preview;
After:
using FluentDragDrop;
using FluentDragDrop.Preview;
using System;
using System.Drawing;
using System.Preview;
*/
using FluentDragDrop.Preview;
using System;
using System.Drawing;
using System.Threading;

namespace FluentDragDropExample
{
    internal class UpdatablePreview : IPreview
    {
        public event EventHandler<Preview> Updated;

        public Bitmap _originalImage;
        private Timer _timer;
        private Preview _preview;
        private readonly Point _mouseStartPosition;

        public UpdatablePreview(Bitmap original, Point mouseStartPosition)
        {
            _originalImage = original ?? throw new ArgumentNullException(nameof(original));
            _mouseStartPosition = mouseStartPosition;

            UpdatePreview(null);
        }

        public Preview Get() => _preview;

        public void Start()
        {
            _timer = new Timer(UpdatePreview, null, TimeSpan.FromMilliseconds(20), TimeSpan.FromMilliseconds(20));
        }

        public void Stop()
        {
            _timer?.Dispose();
            _timer = null;
        }

        private void UpdatePreview(object state)
        {
            var previewImage = new Bitmap(_originalImage);

            var currentMousePosition = System.Windows.Forms.Control.MousePosition;
            var distanceX = Math.Abs(currentMousePosition.X - _mouseStartPosition.X);
            var distanceY = Math.Abs(currentMousePosition.Y - _mouseStartPosition.Y);
            var distance = Math.Round(Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2)));

            using (var graphics = Graphics.FromImage(previewImage))
            {
                using (var font = new Font("Tahoma", 11))
                {
                    using (var format = new StringFormat())
                    {
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Far;

                        var bounds = new Rectangle(0, 20, _originalImage.Width, _originalImage.Height - 40);
                        graphics.DrawString($"Distance: {distance}px", font, Brushes.White, bounds, format);
                    }
                }
            }

            // at 900 distance, we want it to be transparent
            var opacity = (900 - distance) / 900;
            _preview = new Preview(previewImage, opacity);

            Updated?.Invoke(this, _preview);
        }
    }
}