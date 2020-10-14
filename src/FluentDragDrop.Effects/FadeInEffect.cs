using FluentTransitions;
using System;

namespace FluentDragDrop.Effects
{
	/// <summary>
	/// A fade-in effect animating the opacity of the preview image from zero to opaque
	/// </summary>
	public class FadeInEffect : DefaultStartEffect
	{
		/// <summary>
		/// Starts the effect with the given arguments
		/// </summary>
		/// <param name="arguments">The effect arguments containing information about the preview form and the affected controls</param>
		public override void Start(IEffect.Arguments arguments)
		{
			arguments.PreviewForm.Opacity = 0d;
			arguments.PreviewForm.Show();

			Transition
				.With(arguments.PreviewForm, nameof(arguments.PreviewForm.Opacity), 1d)
				.Decelerate(TimeSpan.FromMilliseconds(500));
		}
	}
}
