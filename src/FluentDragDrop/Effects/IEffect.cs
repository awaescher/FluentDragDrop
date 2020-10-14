using System.Windows.Forms;

namespace FluentDragDrop.Effects
{
	/// <summary>
	/// Defines the interface for drag and drop effects
	/// </summary>
	public interface IEffect
	{
		/// <summary>
		/// Starts the effect with the given arguments
		/// </summary>
		/// <param name="arguments">The effect arguments containing information about the preview form and the affected controls</param>
		void Start(Arguments arguments);

		/// <summary>
		/// An arguments container bundling drag and drop effect arguments like the preview form and the affected controls
		/// </summary>
		public class Arguments
		{
			/// <summary>
			/// Creates a new instance of the arguments container
			/// </summary>
			/// <param name="previewForm">The preview form used to display the preview image while dragging</param>
			/// <param name="sourceControl">The control which started the drag and drop operation</param>
			/// <param name="targetControl">The control which received the drop - if any</param>
			public Arguments(Form previewForm, Control sourceControl, Control targetControl)
			{
				PreviewForm = previewForm;
				SourceControl = sourceControl;
				TargetControl = targetControl;
			}

			/// <summary>
			/// The preview form used to display the preview image while dragging
			/// </summary>
			public Form PreviewForm { get; }

			/// <summary>
			/// The control which started the drag and drop operation
			/// </summary>
			public Control SourceControl { get; }

			/// <summary>
			/// The control which received the drop - if any
			/// </summary>
			/// <remarks>Might be null if the drag and drop operation was cancelled</remarks>
			public Control TargetControl { get; }
		}
	}
}