using System;
using System.Windows.Forms;

namespace FluentDragDrop
{
	public abstract class DragDefinition
	{
		public DragDefinition(Control control, DragDropEffects allowedEffects)
		{
			Control = control ?? throw new ArgumentNullException(nameof(control));
			AllowedEffects = allowedEffects;
		}

		internal Control Control { get; }

		internal DragDropEffects AllowedEffects { get; }
	}
}