
namespace FluentDragDrop.Effects
{
	/// <summary>
	/// The default effect to start when a drag and drop operation starts.
	/// This effect simply shows the preview form.
	/// </summary>
	public class DefaultStartEffect : IEffect
	{
		/// <summary>
		/// Starts the effect with the given arguments
		/// </summary>
		/// <param name="arguments">The effect arguments containing information about the preview form and the affected controls</param>
		public virtual void Start(IEffect.Arguments arguments)
		{
			arguments.PreviewForm.Show();
		}
	}
}