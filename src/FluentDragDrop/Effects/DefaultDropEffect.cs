using System.Drawing;

namespace FluentDragDrop.Effects
{
	/// <summary>
	/// The default effect to start when a drag and drop operation was completed successfully.
	/// This effect simply closes the preview form.
	/// </summary>
	public class DefaultDropEffect : IEffect
	{
		/// <summary>
		/// Starts the effect with the given arguments
		/// </summary>
		/// <param name="arguments">The effect arguments containing information about the preview form and the affected controls</param>
		public virtual void Start(IEffect.Arguments arguments)
		{
			arguments.PreviewForm.Close();
			arguments.PreviewForm.Dispose();
		}
	}
}