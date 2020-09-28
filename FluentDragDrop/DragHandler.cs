using System;
using System.Windows.Forms;

namespace FluentDragDrop
{
	public class DragHandler<T>
	{
		public Action<T, DragEventArgs> DragEnter { get; set; }
		public Action<T, DragEventArgs> DragOver { get; set; }
		public Action<T, DragEventArgs> DragDrop { get; set; }
		public Action<T> DragLeave { get; set; }

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
