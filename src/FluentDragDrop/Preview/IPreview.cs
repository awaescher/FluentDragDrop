using System;

namespace FluentDragDrop.Preview
{
    /// <summary>
    /// The interface for previews to be shown during drag and drop operations
    /// </summary>
    public interface IPreview
    {
        /// <summary>
        /// The event to notify the drag and drop operation that the preview was updated
        /// and needs to be redrawn
        /// </summary>
        event EventHandler<PreviewElement> Updated;

        /// <summary>
        /// Starts the preview 
        /// </summary>
        /// <remarks>Can be used to start animated previews</remarks>
        void Start();

        /// <summary>
        /// Stops the preview
        /// </summary>
        /// <remarks>Can be used to stop animated previews</remarks>
        void Stop();

        /// <summary>
        /// Gets the current preview object to show during the drag and drop operation
        /// </summary>
        /// <returns></returns>
        PreviewElement Get();
    }
}