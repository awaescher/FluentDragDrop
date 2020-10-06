using System;

namespace FluentDragDrop.Preview
{
	/// <summary>
	/// The interface for updatable preview which can be changed during a drag and drop operation
	/// </summary>
    public interface IUpdatablePreview : IPreview
    {
		/// <summary>
		/// The event to notify the drag and drop operation that the preview was updated
		/// and needs to be redrawn
		/// </summary>
		event EventHandler Updated;

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
	}
}