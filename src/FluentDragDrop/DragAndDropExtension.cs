using System.Windows.Forms;

namespace FluentDragDrop
{
    /// <summary>
    /// Extension methods for Fluent Drag and Drop
    /// </summary>
    public static class DragAndDropExtension
    {
        /// <summary>
        /// Initializes a drag and drop operation with Fluent Drag and Drop.
        /// Call this method in the MouseDown to correctly implement the Fluent Drag and Drop pattern.
        /// </summary>
        /// <param name="control">The control which starts the drag and drop operation</param>
        /// <returns>Call this method in the MouseDown to correctly implement the Fluent Drag and Drop pattern.</returns>
        public static DragEffectDefinition InitializeDragAndDrop(this Control control)
        {
            return new DragEffectDefinition(control);
        }
    }
}
