using FluentDragDrop;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace FluentDragDropFullFramework
{
	public partial class TestForm : Form
	{
		public TestForm()
		{
			InitializeComponent();
		}

		private void pic1_MouseDown(object sender, MouseEventArgs e)
		{
			var pic = sender as PictureBox;

			// Preview: -
			// Drag style: -

			pic.StartDragAndDrop()
				.WithData(pic.Image)
				.WithoutPreview()
				.To(All, (target, data) => target.Image = data)
				.Copy();
		}

		private void pic2_MouseDown(object sender, MouseEventArgs e)
		{
			var pic = sender as PictureBox;

			// Preview: From Control
			// Drag style: Behind Cursor

			pic.StartDragAndDrop()
				.WithData(pic.Image)
				.WithPreview().BehindCursor()
				.To(All, (target, data) => target.Image = data)
				.Copy();
		}

		private void pic3_MouseDown(object sender, MouseEventArgs e)
		{
			var pic = sender as PictureBox;

			// Image: Custom
			// Drag style: Like Windows Explorer

			pic.StartDragAndDrop()
				.WithData(pic.Image)
				.WithPreview(Grayscale((Bitmap)pic.Image)).LikeWindowsExplorer()
				.To(All, (target, data) => target.Image = data)
				.Copy();
		}

		private void pic4_MouseDown(object sender, MouseEventArgs e)
		{
			var pic = sender as PictureBox;

			// Image: From Control (Watermarked)
			// Drag style: Relative To Cursor

			pic.StartDragAndDrop()
				.WithData(pic.Image)
				.WithPreview(Watermark).RelativeToCursor()
				.To(All, (target, data) => target.Image = data)
				.Copy();
		}

		private Bitmap Grayscale(Bitmap image)
		{
			var result = new Bitmap(image.Width, image.Height);

			using (var graphics = Graphics.FromImage(result))
			{
				var colorMatrix = new ColorMatrix(new float[][]
				{
					new float[] {.3f, .3f, .3f, 0, 0},
					new float[] {.59f, .59f, .59f, 0, 0},
					new float[] {.11f, .11f, .11f, 0, 0},
					new float[] {0, 0, 0, 1, 0},
					new float[] {0, 0, 0, 0, 1}
				});

				using (var attributes = new ImageAttributes())
				{
					attributes.SetColorMatrix(colorMatrix);
					graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
				}
			}
			return result;
		}

		private Bitmap Watermark(Bitmap image)
		{
			using (var graphics = Graphics.FromImage(image))
			{
				using (var font = new Font(Font.FontFamily, 18f))
				{
					using (var format = new StringFormat())
					{
						format.Alignment = StringAlignment.Center;
						format.LineAlignment = StringAlignment.Far;

						var bounds = new Rectangle(Point.Empty, image.Size);
						graphics.DrawString("Dragging ...", font, Brushes.White, bounds, format);
					}
				}
			}

			return image;
		}

		private PictureBox[] All => new[] { pic1, pic2, pic3, pic4, pic5, pic6, pic7, pic8 };

	}
}
