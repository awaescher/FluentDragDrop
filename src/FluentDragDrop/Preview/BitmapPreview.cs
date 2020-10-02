using System;
using System.Drawing;

namespace FluentDragDrop.Preview
{
    internal class BitmapPreview : IPreview
    {
        public event EventHandler<PreviewElement> Updated;

        private readonly PreviewElement _preview;

        public BitmapPreview(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));

            _preview = new PreviewElement(bitmap);
        }

        public PreviewElement Get() => _preview;


        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}
