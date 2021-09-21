using FluentDragDrop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FluentDragDropNuGetExample
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void buttonDrag_MouseDown(object sender, MouseEventArgs e)
        {
            buttonDrag.InitializeDragAndDrop()
                .Link()
                .OnMouseMove()
                .WithData(() => true)
				.WithPreview().RelativeToCursor()
                .To(pnlDrop, (_, __) => MessageBox.Show("Successfully dropped.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information));
        }
    }
}
