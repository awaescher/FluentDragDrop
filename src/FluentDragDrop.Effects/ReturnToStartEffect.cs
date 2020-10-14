using FluentDragDrop.Preview;
using FluentTransitions;
using FluentTransitions.Methods;
using System;
using System.Drawing;

namespace FluentDragDrop.Effects
{
	/// <summary>
	/// An advanced effect returning the preview image to its initial location while fading it out
	/// </summary>
	public class ReturnToStartEffect : DefaultEndEffect
	{
		/// <summary>
		/// Starts the effect with the given arguments
		/// </summary>
		/// <param name="arguments">The effect arguments containing information about the preview form and the affected controls</param>
		public override void Start(IEffect.Arguments arguments)
		{
			var controlLocation = arguments.SourceControl.PointToScreen(Point.Empty);
			var originalOpacity = arguments.PreviewForm.Opacity;

			var returnTransition = Transition
				.With(arguments.PreviewForm, nameof(arguments.PreviewForm.Left), controlLocation.X)
				.With(arguments.PreviewForm, nameof(arguments.PreviewForm.Top), controlLocation.Y)
				.With(arguments.PreviewForm, nameof(arguments.PreviewForm.Opacity), 0d)
				.Build(new Deceleration(500));

			void selfRemovingHandler(object _, Transition.Args __)
			{
				returnTransition.TransitionCompletedEvent -= selfRemovingHandler;

				arguments.PreviewForm.BeginInvoke((Action)(() => base.ClosePreview(arguments)));
			}

			returnTransition.TransitionCompletedEvent += selfRemovingHandler;

			Transition.RunChain(returnTransition);
		}
	}
}
