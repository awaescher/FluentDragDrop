using System.Windows.Forms;

namespace FluentDragDrop
{
	public class ImmediateDragDefinition : DragDefinition
	{
		public ImmediateDragDefinition(Control control, DragDropEffects allowedEffects)
			: base(control, allowedEffects)
		{
		}

		public DragOperation<T> WithData<T>(T data)
		{
			return new DragOperation<T>(this, () => data, () => true);
		}
	}
}