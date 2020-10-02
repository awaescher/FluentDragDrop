using System.Windows.Forms;

namespace FluentDragDrop
{
    /// <summary>
    /// Defines an immediate drag and drop operation on mouse down
    /// </summary>
    public class ImmediateDragDefinition : DragDefinition
    {
        /// <summary>
        /// Creates a new instance of the drag definition
        /// </summary>
        /// <param name="control">The control which starts the drag and drop operation</param>
        /// <param name="effect">The desired drag and drop effect like Copy, Move or Link</param>
        public ImmediateDragDefinition(Control control, DragDropEffects effect)
            : base(control, effect)
        {
        }

        /// <summary>
        /// Defines the data object that should be passed through the drag and drop operation
        /// </summary>
        /// <typeparam name="T">The type of the data object to pass</typeparam>
        /// <param name="data">The data object to pass</param>
        /// <returns></returns>
        public DragOperation<T> WithData<T>(T data)
        {
            return new DragOperation<T>(this, () => data, () => true);
        }
    }
}