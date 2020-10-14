namespace FluentDragDrop.Effects
{
	/// <summary>
	/// An internal container for the three effects to start, drop and cancel drag and drop operations
	/// </summary>
	internal class Effects
	{
		/// <summary>
		/// Get the default effects
		/// </summary>
		/// <returns></returns>
		public static Effects GetDefaults()
		{
			return new Effects
			{
				StartEffect = new DefaultStartEffect(),
				DropEffect = new DefaultEndEffect(),
				CancelEffect = new DefaultEndEffect()
			};
		}

		/// <summary>
		/// The effect to start at the beginning of the drag and drop operation
		/// </summary>
		public IEffect StartEffect { get; set; }

		/// <summary>
		/// The effect to start when the drag and drop operation was completed successfully
		/// </summary>
		public IEffect DropEffect { get; set; }

		/// <summary>
		/// The effect to start when the drag and drop operation was cancelled
		/// </summary>
		public IEffect CancelEffect { get; set; }
	}
}
