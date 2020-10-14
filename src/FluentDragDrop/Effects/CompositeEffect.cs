using System;

namespace FluentDragDrop.Effects
{
	/// <summary>
	/// An effect to bundle and start multiple instances of other effects at the same time
	/// </summary>
	public class CompositeEffect : IEffect
	{
		/// <summary>
		/// Creates a new composite effect to bundle further effects to start simultaneously
		/// </summary>
		/// <param name="effects">The bundled effects to start</param>
		public CompositeEffect(params IEffect[] effects)
		{
			Effects = effects ?? throw new ArgumentNullException(nameof(effects));
		}

		/// <summary>
		/// Starts the effect with the given arguments
		/// </summary>
		/// <param name="arguments">The effect arguments containing information about the preview form and the affected controls</param>
		public void Start(IEffect.Arguments arguments)
		{
			foreach (var effect in Effects)
				effect.Start(arguments);
		}

		/// <summary>
		/// The effects to start simultaneously
		/// </summary>
		public IEffect[] Effects { get; }
	}
}