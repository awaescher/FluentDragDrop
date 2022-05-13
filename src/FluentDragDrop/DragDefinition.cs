using System;
using System.Windows.Forms;

namespace FluentDragDrop
{
    /// <summary>
    /// The drag definition containing the control which starts the drag and drop operation and the desired effect
    /// like Copy, Move or Link.
    /// </summary>
    public abstract class DragDefinition
    {
        /// <summary>
        /// Creates a new drag definion
        /// </summary>
        /// <param name="control">The control which starts the drag and drop operation</param>
        /// <param name="effect">The desired drag and drop effect like Copy, Move or Link</param>
        protected DragDefinition(Control control, DragDropEffects effect)
        {
            Control = control ?? throw new ArgumentNullException(nameof(control));
            Effect = effect;
        }

        /// <summary>
        /// Gets the control which starts the drag and drop operation
        /// </summary>
        internal Control Control { get; private set; }

        /// <summary>
        /// Gets the desired drag and drop effect like Copy, Move or Link
        /// </summary>
        internal DragDropEffects Effect { get; private set; }
    }
}