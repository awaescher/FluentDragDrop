using System.Windows.Forms;

namespace FluentDragDrop
{
	public static class DragAndDropExtension
	{
		public static DragDefinition StartDragAndDrop(this Control control)
		{
			return new DragDefinition(control);
		}
	}
}
