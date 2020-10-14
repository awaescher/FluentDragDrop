using System.Drawing;

namespace FluentDragDrop.Preview
{
    /// <summary>
    /// The interface for previews to be shown during drag and drop operations
    /// </summary>
    public interface IPreview
    {
		/// <summary>
		/// Renders the preview onto the graphics object of the preview element
		/// </summary>
		/// <param name="graphics">The graphics object of the preview element</param>
        void Render(Graphics graphics);

		/// <summary>
		/// Gets the preferred size of the preview
		/// </summary>
		/// <returns></returns>
        Size PreferredSize { get; }
	}
}