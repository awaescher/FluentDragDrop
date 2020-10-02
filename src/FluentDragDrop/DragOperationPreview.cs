using System;
using System.Drawing;
using System.Windows.Forms;

namespace FluentDragDrop
{
    /// <summary>
    /// Defines how the preview should be attached to the mouse cursor
    /// </summary>
    /// <typeparam name="T">The type of the data to drop</typeparam>
    public class DragOperationPreview<T>
    {
        private readonly DragOperation<T> _operation;

        /// <summary>
        /// Creates a new instance of the operation preview
        /// </summary>
        /// <param name="operation">The drag and drop operation</param>
        public DragOperationPreview(DragOperation<T> operation)
        {
            _operation = operation ?? throw new ArgumentNullException(nameof(operation));
        }

        /// <summary>
        /// Attaches the preview to the bottom right of the cursor so that its upper left corner is next to the mouse cursor.
        /// </summary>
        /// <returns></returns>
        public DragOperation<T> BehindCursor()
        {
            return _operation.WithCursorOffset(Cursor.Current.Size.Width / 4, Cursor.Current.Size.Height / 4);
        }

        /// <summary>
        /// Attaches the preview relatively to the position of the cursor to the control.
        /// This is the most natural mode when dragging the preview of the control itself in original size as it feels like the control is dragged directly.
        /// </summary>
        /// <returns></returns>
        public DragOperation<T> RelativeToCursor()
        {
            var mousePositionOnScreen = Control.MousePosition;
            var sourceControlPositionOnScreen = _operation.SourceControl.PointToScreen(Point.Empty);

            var offsetX = -1 * (mousePositionOnScreen.X - sourceControlPositionOnScreen.X);
            var offsetY = -1 * (mousePositionOnScreen.Y - sourceControlPositionOnScreen.Y);

            return _operation.WithCursorOffset(offsetX, offsetY);
        }

        /// <summary>
        /// Attaches the preview above the cursor like the Windows Explorer shows previews while drag and drop operations of files and folders
        /// </summary>
        /// <returns></returns>
        public DragOperation<T> LikeWindowsExplorer()
        {
            var previewSize = _operation.CalculatePreviewSize();

            var offsetX = -1 * (previewSize.Width / 2);
            var offsetY = -1 * (previewSize.Height - (Cursor.Current.Size.Height / 2));

            return _operation.WithCursorOffset(offsetX, offsetY);
        }
    }
}