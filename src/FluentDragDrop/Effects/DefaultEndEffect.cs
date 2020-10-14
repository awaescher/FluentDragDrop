using System.Drawing;
using System.Windows.Forms;

namespace FluentDragDrop.Effects
{
	/// <summary>
	/// The default effect to end a drag and drop.
	/// This effect simply closes the preview form.
	/// </summary>
	public class DefaultEndEffect : IEffect
	{
		/// <summary>
		/// Starts the effect with the given arguments
		/// </summary>
		/// <param name="arguments">The effect arguments containing information about the preview form and the affected controls</param>
		public virtual void Start(IEffect.Arguments arguments)
		{
			ClosePreview(arguments);
		}

		/// <summary>
		/// Closes and disposes the preview form
		/// </summary>
		/// <param name="arguments">The effect arguments containing information about the preview form and the affected controls</param>
		protected virtual void ClosePreview(IEffect.Arguments arguments)
		{
			arguments.PreviewForm.Close();
			arguments.PreviewForm.Dispose();
		}
	}
}