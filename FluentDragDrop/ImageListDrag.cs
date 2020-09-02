using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace FluentDragDrop
{
	/// <summary>
	/// Provides the ability to create 32-bit alpha drag images
	/// using the ImageList drag functionality in .NET.
	/// </summary>
	/// <remarks></remarks>
	internal class ImageListDrag : IDisposable
	{
		[StructLayout(LayoutKind.Sequential)]
		private struct POINTAPI
		{
			public Int32 X;
			public Int32 Y;

			public override string ToString()
			{
				return $"{this.GetType().FullName} X={X},Y={Y}";
			}
		}

		[DllImport("comctl32")]
		private static extern Int32 ImageList_BeginDrag(IntPtr himlTrack, Int32 iDrag, Int32 dxHotspot, Int32 dyHotspot);

		[DllImport("comctl32")]
		private static extern void ImageList_EndDrag();

		[DllImport("comctl32")]
		private static extern Int32 ImageList_DragEnter(IntPtr hwndLock, Int32 x, Int32 y);

		[DllImport("comctl32")]
		private static extern Int32 ImageList_DragLeave(IntPtr hwndLock);

		[DllImport("comctl32")]
		private static extern Int32 ImageList_DragMove(Int32 x, Int32 y);

		/// <summary>
		/// 	The Window handle which we're dragging over.
		/// 	</summary>
		/// 	<remarks></remarks>
		private IntPtr _HWndLast = IntPtr.Zero;

		/// <summary>
		/// 	Whether dragging is occurring or not.
		/// 	</summary>
		/// 	<remarks></remarks>
		private bool _inDrag = false;

		/// <summary>
		/// 	Whether we have suspended image
		/// 	dragging and need to start it again when the
		/// 	cursor next moves.
		/// 	</summary>
		/// 	<remarks></remarks>
		private bool _startDrag = false;

		/// <summary>
		/// 	Constructs a new instance of the ImageListDrag class.
		/// 	</summary>
		/// 	<remarks></remarks>
		public ImageListDrag()
		{
		}

		/// <summary>
		/// 	Clears up any resources associated with this object.
		/// 	Note there are only resources associated when there is
		/// 	a drag operation in effect.
		/// 	</summary>
		/// 	<remarks></remarks>
		public void Dispose()
		{
			CompleteDrag();
		}

		/// <summary>
		/// 	Starts a dragging operation which will use
		/// 	an ImageList to create a drag image and defaults
		/// 	the position of the image to the cursor's drag point
		/// 	</summary>
		public void StartDrag()
		{
			StartDrag(0);
		}

		/// <summary>
		/// 	Starts a dragging operation which will use
		/// 	an ImageList to create a drag image and defaults
		/// 	the position of the image to the cursor's drag point
		/// 	</summary>
		/// 	<param name="dragImage">The image used for dragging</param>
		public void StartDrag(Bitmap dragImage)
		{
			SetDragImage(dragImage);
			StartDrag(0);
		}

		/// <summary>
		/// 	Starts a dragging operation which will use
		/// 	an ImageList to create a drag image and allows
		/// 	the offset of the Image from the drag position
		/// 	to be specified.
		/// 	</summary>
		/// 	<param name="dragImage">The image used for dragging</param>
		/// 	<param name="xOffset">The horizontal offset of the drag image
		/// 	from the drag position.  Negative values move the image
		/// 	to the right of the cursor, positive values move it
		/// 	to the left.</param>
		/// 	<param name="yOffset">>The vertical offset of the drag image
		/// 	from the drag position. Negative values move the image
		/// 	below the cursor, positive values move it above.</param>
		/// 	<remarks></remarks>
		public void StartDrag(Bitmap dragImage, Int32 xOffset, Int32 yOffset)
		{
			SetDragImage(dragImage);
			StartDrag(0, xOffset, yOffset);
		}

		/// <summary>
		/// 	Starts a dragging operation which will use
		/// 	an ImageList to create a drag image and defaults
		/// 	the position of the image to the cursor's drag point
		/// 	</summary>
		/// 	<param name="imageIndex">The index of the image in
		/// 	the ImageList to use for the drag image.</param>
		/// 	<remarks></remarks>
		public void StartDrag(Int32 imageIndex)
		{
			StartDrag(imageIndex, 0, 0);
		}

		/// <summary>
		/// 	Starts a dragging operation which will use
		/// 	an ImageList to create a drag image and allows
		/// 	the offset of the Image from the drag position
		/// 	to be specified.
		/// 	</summary>
		/// 	<param name="imageIndex">The index of the image in the ImageList to use for the drag image.</param>
		/// 	<param name="xOffset">The horizontal offset of the drag image
		/// 	from the drag position.  Negative values move the image
		/// 	to the right of the cursor, positive values move it
		/// 	to the left.</param>
		/// 	<param name="yOffset">>The vertical offset of the drag image
		/// 	from the drag position. Negative values move the image
		/// 	below the cursor, positive values move it above.</param>
		/// 	<remarks></remarks>
		public void StartDrag(Int32 imageIndex, Int32 xOffset, Int32 yOffset)
		{
			Int32 res = 0;
			CompleteDrag();
			res = ImageList_BeginDrag(Imagelist.Handle, imageIndex, xOffset, yOffset);
			if ((res != 0))
			{
				_inDrag = true;
				_startDrag = true;
			}
		}

		/// <summary>
		/// 	Shows the ImageList drag image at the current dragging position.
		/// 	</summary>
		/// 	<remarks></remarks>
		public void DragDrop()
		{
			Point dst = new Point();

			if ((_inDrag))
			{
				dst = Cursor.Position;
				if ((Owner != null))
					// Position relative to owner:
					dst = Owner.PointToClient(dst);

				if ((_startDrag))
				{
					_HWndLast = Owner == null ? IntPtr.Zero : Owner.Handle;
					ImageList_DragEnter(_HWndLast, dst.X, dst.Y);
					_startDrag = false;
				}

				ImageList_DragMove(dst.X, dst.Y);
			}
		}

		/// <summary>
		/// 	Completes a drag operation.
		/// 	</summary>
		/// 	<remarks></remarks>
		public void CompleteDrag()
		{
			if ((_inDrag))
			{
				ImageList_EndDrag();
				ImageList_DragLeave(_HWndLast);
				_HWndLast = IntPtr.Zero;
				_inDrag = false;
			}
		}

		/// <summary>
		/// 	Shows or hides the drag image.  This is used to prevent
		/// 	painting problems if the area under the drag needs to
		/// 	be repainted.
		/// 	</summary>
		/// 	<param name="state">True to hide the drag image and
		/// 	allow repainting, False to show the drag image.</param>
		/// 	<remarks></remarks>
		public void HideDragImage(bool state)
		{
			if ((_inDrag))
			{
				if ((state))
				{
					ImageList_DragLeave(_HWndLast);
					_startDrag = true;
				}
				else
					DragDrop();
			}
		}

		/// <summary>
		/// 	Draws and adds a new drag image
		/// 	</summary>
		/// 	<param name="drawAction">Action which is executed to draw to the drag image</param>
		/// 	<remarks></remarks>
		public void DrawDragImage(Action<Graphics, Bitmap> drawAction)
		{

			// Clear images in image list:
			Imagelist.Images.Clear();

			// ImageList is buggy, need to ensure we do this:
			IntPtr ilsHandle = Imagelist.Handle;

			// Create the bitmap to hold the drag image:
			using (Bitmap bitmap = new Bitmap(Imagelist.ImageSize.Width, Imagelist.ImageSize.Height))
			{
				// Get a graphics object from it:
				using (Graphics gfx = Graphics.FromImage(bitmap))
				{
					drawAction(gfx, bitmap);

					// Add the image to the ImageList:
					Imagelist.Images.Add(bitmap, Color.Fuchsia);
				}
			}
		}

		/// <summary>
		/// 	Adds the drag image
		/// 	</summary>
		/// 	<param name="bitmap">Bitmap used as drag image</param>
		/// 	<remarks></remarks>
		public void SetDragImage(Bitmap bitmap)
		{
			SetDragImage(bitmap, Color.Fuchsia);
		}

		/// <summary>
		/// 	Adds the drag image
		/// 	</summary>
		/// 	<param name="bitmap">Bitmap used as drag image</param>
		/// 	<param name="transparentColor">The color used for transparent areas</param>
		/// 	<remarks></remarks>
		public void SetDragImage(Bitmap bitmap, Color transparentColor)
		{
			Imagelist = new ImageList();
			Imagelist.ImageSize = bitmap.Size;

			// ImageList is buggy, need to ensure we do this:
			IntPtr ilsHandle = Imagelist.Handle;

			// Add the image to the ImageList:
			Imagelist.Images.Add(bitmap, transparentColor);
		}

		/// <summary>
		/// 	Gets/sets the ImageList used to source drag images.
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		public System.Windows.Forms.ImageList Imagelist { get; set; } = null/* TODO Change to default(_) if this is not a reference type */;

		/// <summary>
		/// 	Gets/sets the Owning control or form for this object.
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		public Control Owner { get; set; } = null /* TODO Change to default(_) if this is not a reference type */;

		/// <summary>
		/// 	Gets the default instance
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		public static ImageListDrag Default { get; } = new ImageListDrag();
	}
}