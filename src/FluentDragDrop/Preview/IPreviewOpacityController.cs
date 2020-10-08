using System.Drawing;

namespace FluentDragDrop.Preview
{
    /// <summary>
    /// The interface for previews to be shown during drag and drop operations
    /// </summary>
    public interface IPreviewOpacityController
    {
		/// <summary>
		/// Gets the opacity of the preview
		/// </summary>
        double Opacity { get; }

		/// <summary>
		/// Gets the color which should be used as transparency key.
		/// Each pixel in this color is transparent.
		/// Use Color.Empty to 
		/// </summary>
		Color TransparencyKey { get; }
	}
}