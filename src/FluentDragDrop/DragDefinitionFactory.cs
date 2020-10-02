using System;
using System.Windows.Forms;

namespace FluentDragDrop
{
    /// <summary>
    /// A factory class to instantiate drag definitions
    /// </summary>
    public class DragDefinitionFactory
    {
        /// <summary>
        /// Creates a new drag definition factory to instantiate drag definitions
        /// </summary>
        /// <param name="control">The control which starts the drag and drop operation</param>
        public DragDefinitionFactory(Control control)
        {
            Control = control ?? throw new ArgumentNullException(nameof(control));
        }

        /// <summary>
        /// Starts the drag and drop operation immediately on mouse down which might work very well with static controls
        /// but not with controls providing selection mechanisms like ListBoxes, ListViews, Grids, etc.
        /// If you want to drag the selected items of a ListView for example, use OnMouseMove() instead.
        /// </summary>
        /// <returns></returns>
        /// <remarks>If you want to drag the selected items on a ListView for example, use OnMouseMove() instead.</remarks>
        public ImmediateDragDefinition Immediately() => new ImmediateDragDefinition(Control, Effect);

        /// <summary>
        /// Captures the mouse events of the target control(s) and starts the drag and drop operation as soon as the mouse
        /// is moved while the left mouse button is still pressed.
        /// Prefer this mode over Immediately() if you want to drag the selected items of a ListView for example because the selection 
        /// is made after the mouse down event. By using Immediately() the drag and drop operation might be started before the selection is made.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Prefer this mode over Immediately() if you want to drag the selected items of a ListView for example because the selection 
        /// is made after the mouse down event. By using Immediately() the drag and drop operation might be started before the selection is made.
        /// </remarks>
        public DelayedDragDefinition OnMouseMove() => new DelayedDragDefinition(Control, Effect);

        /// <summary>
        /// Gets the control which starts the drag and drop operation
        /// </summary>
        internal Control Control { get; }

        /// <summary>
        /// Gets or sets the desired drag and drop effect like Copy, Move or Link
        /// </summary>
        internal DragDropEffects Effect { get; set; } = DragDropEffects.None;
    }
}
