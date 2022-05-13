using System.Windows.Forms;

namespace FluentDragDrop
{
    /// <summary>
    /// The definition class for drag effects like Copy, Move or Link
    /// </summary>
    public class DragEffectDefinition
    {
        private readonly DragDefinitionFactory _definitionFactory;

        /// <summary>
        /// Creates a new drag effect definition instance
        /// </summary>
        /// <param name="control">The control which starts the drag and drop operation</param>
        public DragEffectDefinition(Control control)
        {
            _definitionFactory = new DragDefinitionFactory(control);
        }

        /// <summary>
        /// Defines the drag and drop effect "Copy"
        /// </summary>
        public DragDefinitionFactory Copy()
        {
            _definitionFactory.Effect = DragDropEffects.Copy;
            return _definitionFactory;
        }

        /// <summary>
        /// Defines the drag and drop effect "Link"
        /// </summary>
        public DragDefinitionFactory Link()
        {
            _definitionFactory.Effect = DragDropEffects.Link;
            return _definitionFactory;
        }

        /// <summary>
        /// Defines the drag and drop effect "Move"
        /// </summary>
        public DragDefinitionFactory Move()
        {
            _definitionFactory.Effect = DragDropEffects.Move;
            return _definitionFactory;
        }
    }
}