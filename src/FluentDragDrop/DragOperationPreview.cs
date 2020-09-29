using System;
using System.Drawing;
using System.Windows.Forms;

namespace FluentDragDrop
{
    public class DragOperationPreview<T>
    {
        public DragOperationPreview(DragOperation<T> operation)
        {
            Operation = operation ?? throw new ArgumentNullException(nameof(operation));
        }

        public DragOperation<T> BehindCursor()
        {
            return Operation.WithCursorOffset(Cursor.Current.Size.Width / 4, Cursor.Current.Size.Height / 4);
        }

        public DragOperation<T> RelativeToCursor()
        {
            var mousePositionOnScreen = Control.MousePosition;
            var sourceControlPositionOnScreen = Operation.SourceControl.PointToScreen(Point.Empty);

            var offsetX = -1 * (mousePositionOnScreen.X - sourceControlPositionOnScreen.X);
            var offsetY = -1 * (mousePositionOnScreen.Y - sourceControlPositionOnScreen.Y);

            return Operation.WithCursorOffset(offsetX, offsetY);
        }

        public DragOperation<T> LikeWindowsExplorer()
        {
            var previewSize = Operation.CalculatePreviewSize();

            var offsetX = -1 * (previewSize.Width / 2);
            var offsetY = -1 * (previewSize.Height - (Cursor.Current.Size.Height / 2));

            return Operation.WithCursorOffset(offsetX, offsetY);
        }

        public DragOperation<T> Operation { get; }
    }
}
