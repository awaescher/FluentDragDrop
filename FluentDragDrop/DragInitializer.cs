using System.Windows.Forms;

namespace FluentDragDrop
{
	public class DragInitializer
	{
		public DragInitializer(Control control)
		{
			DefinitionFactory = new DragDefinitionFactory(control);
		}

		public DragDefinitionFactory Copy()
		{
			DefinitionFactory.AllowEffects(DragDropEffects.Copy);
			return DefinitionFactory; 
		}

		public DragDefinitionFactory Link()
		{
			DefinitionFactory.AllowEffects(DragDropEffects.Link);
			return DefinitionFactory;
		}

		public DragDefinitionFactory Move() 
		{
			DefinitionFactory.AllowEffects(DragDropEffects.Move);
			return DefinitionFactory; 
		}

		private DragDefinitionFactory DefinitionFactory { get; }
	
	
	}
}
