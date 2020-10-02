using System;
using System.Windows.Forms;

namespace FluentDragDrop
{
    /// <summary>
    /// A handler class providing hooks to implement the code on drag events of target controls
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DragHandler<T>
    {
        /// <summary>
        /// Gets or sets the action hook for the DragEnter event of a control
        /// </summary>
        public Action<T, DragEventArgs> DragEnter { get; set; }

        /// <summary>
        /// Gets or sets the action hook for the DragOver event of a control
        /// </summary>
        public Action<T, DragEventArgs> DragOver { get; set; }

        /// <summary>
        /// Gets or sets the action hook for the DragDrop event of a control
        /// </summary>
        public Action<T, DragEventArgs> DragDrop { get; set; }

        /// <summary>
        /// Gets or sets the action hook for the DragLeave event of a control
        /// </summary>
        public Action<T> DragLeave { get; set; }

        /// <summary>
        /// Creates a handler instance with default hook implementations
        /// </summary>
        /// <returns></returns>
        public static DragHandler<T> CreateDefault()
        {
            return new DragHandler<T>
            {
                DragEnter = (_, args) => args.Effect = args.AllowedEffect,
                DragOver = (_, args) => args.Effect = args.AllowedEffect,
                DragDrop = null,
                DragLeave = null,
            };
        }
    }
}
