using System.Windows.Forms;

namespace FluentDragDrop
{
	public static class DragAndDropExtension
	{
		public static DragInitializer InitializeDragAndDrop(this Control control)
		{
			return new DragInitializer(control);
		}
	}
}
