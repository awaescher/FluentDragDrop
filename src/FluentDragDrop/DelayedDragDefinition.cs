using System;
using System.Windows.Forms;

namespace FluentDragDrop
{
    /// <summary>
    /// Defines a delayed drag and drop operation on mouse movement
    /// </summary>
    public class DelayedDragDefinition : DragDefinition
    {
        private Func<bool> _conditionEvaluator;

        /// <summary>
        /// Creates a new instance of the drag definition
        /// </summary>
        /// <param name="control">The control which starts the drag and drop operation</param>
        /// <param name="effect">The desired drag and drop effect like Copy, Move or Link</param>
        public DelayedDragDefinition(Control control, DragDropEffects effect)
            : base(control, effect)
        {
            _conditionEvaluator = () => true;
        }

        /// <summary>
        /// Defines a condition which has to return true to start the drag and drop operation as
        /// soon as mouse movement is detected. If this condition returns false, the drag and drop
        /// operation will not be started.
        /// </summary>
        /// <param name="conditionEvaluator">The condition whether the drag and drop operation should be started or not</param>
        /// <returns></returns>
        public DelayedDragDefinition If(Func<bool> conditionEvaluator)
        {
            _conditionEvaluator = conditionEvaluator;
            return this;
        }

        /// <summary>
        /// Defines the data object that should be passed through the drag and drop operation
        /// </summary>
        /// <typeparam name="T">The type of the data object to pass</typeparam>
        /// <param name="dataEvaluator">A function to retrieve the data object to pass through the drag and drop operation</param>
        /// <returns></returns>
        public DragOperation<T> WithData<T>(Func<T> dataEvaluator)
        {
            return new DragOperation<T>(this, dataEvaluator, _conditionEvaluator);
        }
    }
}