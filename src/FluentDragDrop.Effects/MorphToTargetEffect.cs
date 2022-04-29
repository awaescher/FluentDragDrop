using FluentDragDrop.Preview;
using FluentTransitions;
using FluentTransitions.Methods;
using System;
using System.Drawing;

namespace FluentDragDrop.Effects
{
	/// <summary>
	/// A combined effect moving the preview image to the target control while fading it out.
	/// </summary>
	public class MorphToTargetEffect : DefaultEndEffect
	{
		/// <summary>
		/// Starts the effect with the given arguments
		/// </summary>
		/// <param name="arguments">The effect arguments containing information about the preview form and the affected controls</param>
		public override void Start(IEffect.Arguments arguments)
		{
			if (arguments?.TargetControl is null)
				throw new ArgumentNullException($"Target control cannot be null for this {nameof(MorphToTargetEffect)}");

			var screenLocationOfSourceControl = arguments.TargetControl.PointToScreen(Point.Empty);

			var centerOfTargetControl = new Point(
				screenLocationOfSourceControl.X + (arguments.TargetControl.Width / 2),
				screenLocationOfSourceControl.Y + (arguments.TargetControl.Height / 2));

			var previewTransitionLocation = new Point(
				centerOfTargetControl.X - (arguments.PreviewForm.Width / 2),
				centerOfTargetControl.Y - (arguments.PreviewForm.Height / 2));

			var transition = Transition
				.With(arguments.PreviewForm, nameof(arguments.PreviewForm.Left), previewTransitionLocation.X)
				.With(arguments.PreviewForm, nameof(arguments.PreviewForm.Top), previewTransitionLocation.Y)
				.With(arguments.PreviewForm, nameof(arguments.PreviewForm.Opacity), 0d)
				.Build(new Deceleration(750));

			void selfRemovingHandler(object _, EventArgs __)
			{
				transition.TransitionCompleted -= selfRemovingHandler;

				arguments.PreviewForm.BeginInvoke((Action)(() => base.ClosePreview(arguments)));
			}

			transition.TransitionCompleted += selfRemovingHandler;

			transition.Run();
		}
	}
}