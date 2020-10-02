using System.Drawing;

namespace FluentDragDrop.Preview
{
    /// <summary>
    /// A preview container holding the preview bitmap and its opacity
    /// </summary>
    public class PreviewElement
    {
        /// <summary>
        /// Creates a new preview container with a given bitmap
        /// </summary>
        /// <param name="bitmap">The bitmap to show during the drag and drop operation</param>
        public PreviewElement(Bitmap bitmap)
        {
            Bitmap = bitmap;
        }

        /// <summary>
        /// Creates a new preview container with a given bitmap
        /// </summary>
        /// <param name="bitmap">The bitmap to show during the drag and drop operation</param>
        /// <param name="opacity">The opacity of the preview during the drag and drop operation</param>
        public PreviewElement(Bitmap bitmap, double opacity)
        {
            Bitmap = bitmap;
            Opacity = opacity;
        }

        /// <summary>
        /// Gets or sets the bitmap to show during the drag and drop operation
        /// </summary>
        public Bitmap Bitmap { get; set; }

        /// <summary>
        /// Gets or sets the opacity of the preview during the drag and drop operation
        /// </summary>
        public double Opacity { get; set; } = 0.8;
    }
}