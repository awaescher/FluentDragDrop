using System;
using System.Windows.Forms;

namespace FluentDragDrop
{
	public class DragDefinitionFactory
	{
		public DragDefinitionFactory(Control control)
		{
			Control = control ?? throw new ArgumentNullException(nameof(control));
		}

		public ImmediateDragDefinition Immediately() => new ImmediateDragDefinition(Control, AllowedEffects);

		public DelayedDragDefinition OnMouseMove() => new DelayedDragDefinition(Control, AllowedEffects);

		internal void AllowEffects(DragDropEffects effects) => AllowedEffects |= effects;

		public Control Control { get; }

		public DragDropEffects AllowedEffects { get; private set; } = DragDropEffects.None;
	}
}
