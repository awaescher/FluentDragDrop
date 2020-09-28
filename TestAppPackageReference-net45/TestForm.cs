using FluentDragDrop;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TestAppPackageReference
{
	public partial class TestForm : Form
	{
		public TestForm()
		{
			InitializeComponent();

			var label1 = new Label
			{
				BorderStyle = BorderStyle.FixedSingle,
				Text = "Drag me",
				TextAlign = ContentAlignment.MiddleCenter
			};
			label1.Location = new Point(50, 50);
			label1.Size = new Size(240, 80);

			Controls.Add(label1);
			label1.Show();

			var button1 = new Button
			{
				Text = "Here"
			};
			button1.Location = new Point(400, 50);
			button1.Size = label1.Size;

			Controls.Add(button1);
			button1.Show();

			label1.MouseDown += Label1_MouseDown;
		}

		private void Label1_MouseDown(object sender, MouseEventArgs e)
		{
			var label = sender as Label;

			label.InitializeDragAndDrop()
				.Copy()
				.Immediately()
				.WithData(label.Text)
				.WithPreview().RelativeToCursor()
				.To(Controls.OfType<Button>(), (target, data) => MessageBox.Show("Data dropped.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information));
		}
	}
}
