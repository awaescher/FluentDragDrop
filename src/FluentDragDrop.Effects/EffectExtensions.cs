namespace FluentDragDrop.Effects
{
	/// <summary>
	/// Provides a set of extension methods to add effects in a fluent syntax
	/// </summary>
	public static class EffectExtensions
	{
		/// <summary>
		/// Adds a fade-in effect as start effect to a given drag and drop operation.
		/// </summary>
		/// <typeparam name="T">The type of the data to drop</typeparam>
		/// <param name="dragOperation">The drag and drop operation to add the effect to</param>
		/// <returns></returns>
		public static DragOperation<T> FadeInOnStart<T>(this DragOperation<T> dragOperation)
		{
			return dragOperation.WithStartEffects(new FadeInEffect());
		}

		/// <summary>
		/// Adds a fade-out effect as drop effect for completed drag and drop operations.
		/// </summary>
		/// <typeparam name="T">The type of the data to drop</typeparam>
		/// <param name="dragOperation">The drag and drop operation to add the effect to</param>
		/// <returns></returns>
		public static DragOperation<T> FadeOutOnDrop<T>(this DragOperation<T> dragOperation)
		{
			return dragOperation.WithDropEffects(new FadeOutEffect());
		}

		/// <summary>
		/// Adds a fade-out effect as effect for a cancelled drag and drop operation.
		/// </summary>
		/// <typeparam name="T">The type of the data to drop</typeparam>
		/// <param name="dragOperation">The drag and drop operation to add the effect to</param>
		/// <returns></returns>
		public static DragOperation<T> FadeOutOnCancel<T>(this DragOperation<T> dragOperation)
		{
			return dragOperation.WithCancelEffects(new FadeOutEffect());
		}

		/// <summary>
		/// Adds a combined effect moving the preview image to its starting point while fading out as effect for a cancelled drag and drop operation.
		/// </summary>
		/// <typeparam name="T">The type of the data to drop</typeparam>
		/// <param name="dragOperation">The drag and drop operation to add the effect to</param>
		/// <returns></returns>
		public static DragOperation<T> ReturnToStartOnCancel<T>(this DragOperation<T> dragOperation)
		{
			return dragOperation.WithCancelEffects(new ReturnToStartEffect());
		}
	}
}
