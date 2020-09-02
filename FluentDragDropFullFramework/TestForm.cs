using System.Drawing;
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
			// Drag: -

			pic.StartDragAndDrop()
				.WithData(pic.Image)
				.WithoutPreview()
				.To(All, (p, data) => p.Image = data)
				.Copy();
		}

		private void pic2_MouseDown(object sender, MouseEventArgs e)
		{
			var pic = sender as PictureBox;

			// Preview: From Control
			// Drag: At Cursor

			pic.StartDragAndDrop()
				.WithData(pic.Image)
				.WithPreview().BehindCursor()
				.To(All, (p, data) => p.Image = data)
				.Copy();
		}

		private void pic3_MouseDown(object sender, MouseEventArgs e)
		{
			var pic = sender as PictureBox;

			// Image: From Control
			// Drag: Behind Cursor

			pic.StartDragAndDrop()
				.WithData(pic.Image)
				.WithPreview((Bitmap)pic.Image).LikeWindowsExplorer()
				.To(All, (p, data) => p.Image = data)
				.Copy();
		}

		private void pic4_MouseDown(object sender, MouseEventArgs e)
		{
			var pic = sender as PictureBox;

			// Image: Custom
			// Drag: Direct Grab

			pic.StartDragAndDrop()
				.WithData(pic.Image)
				.WithPreview(Watermark).RelativeToCursor()
				.To(All, (p, data) => p.Image = data)
				.Copy();
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
						graphics.DrawString("Drag & Drop", font, Brushes.White, bounds, format);
					}
				}
			}

			return image;
		}

		private PictureBox[] All => new[] { pic1, pic2, pic3, pic4, pic5, pic6, pic7, pic8 };

	}
}
