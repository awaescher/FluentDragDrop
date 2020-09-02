using System;
using System.Windows.Forms;

namespace FluentDragDrop
{
	public class DragDefinition
	{
		public DragDefinition(Control control)
		{
			this.Control = control ?? throw new ArgumentNullException(nameof(control));
		}

		public DragOperation<T> WithData<T>(T data)
		{
			return new DragOperation<T>(Control, data);
		}

		private Control Control { get; }
	}
}
