using FluentTransitions;
using FluentTransitions.Methods;
using System;

namespace FluentDragDrop.Effects
{
	/// <summary>
	/// A fade-out effect fading the opacity of the preview image from its current value to zero
	/// </summary>
	public class FadeOutEffect : DefaultEndEffect
	{
		/// <summary>
		/// Starts the effect with the given arguments
		/// </summary>
		/// <param name="arguments">The effect arguments containing information about the preview form and the affected controls</param>
		public override void Start(IEffect.Arguments arguments)
		{
			var transition = Transition
				.With(arguments.PreviewForm, nameof(arguments.PreviewForm.Opacity), 0d)
				.Build(new Deceleration(500));

			void selfRemovingHandler(object _, Transition.Args __)
			{
				transition.TransitionCompletedEvent -= selfRemovingHandler;

				arguments.PreviewForm.BeginInvoke((Action)(() => base.ClosePreview(arguments)));
			}

			transition.TransitionCompletedEvent += selfRemovingHandler;

			transition.Run();
		}
	}
}
