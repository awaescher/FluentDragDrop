using FluentDragDrop;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace FluentDragDropExample
{
    public partial class TestForm : Form
    {
        private bool _listCompatibilityTraditionalMouseDown;

        public TestForm()
        {
            InitializeComponent();
        }

        private void picNoPreview_MouseDown(object sender, MouseEventArgs e)
        {
            var pic = (PictureBox)sender;
            pic.InitializeDragAndDrop()
                .Copy()
                .Immediately()
                .WithData(pic.Image)
                .WithoutPreview()
                .To(PreviewBoxes, (target, data) => target.Image = data);
        }

        #region Usage
        private void picControlPreviewBehindCursor_MouseDown(object sender, MouseEventArgs e)
        {
            var pic = (PictureBox)sender;

            pic.InitializeDragAndDrop()
                .Copy()
                .Immediately()
                .WithData(pic.Image)
                .WithPreview().BehindCursor()
                .To(PreviewBoxes, (target, data) => target.Image = data);

            // Copy(), Move() or Link() to define allowed effects
            // Immediately() or OnMouseMove() for deferred start on mouse move
            // WithData() to pass any object you like
            // WithPreview() to define your preview and how it should behave
            //     BehindCursor() or RelativeToCursor() to define the preview placement
            // To() to define target controls and how the dragged data should be used on drop
        }
        #endregion

        private void picCustomPreviewWindowsExplorerStyle_MouseDown(object sender, MouseEventArgs e)
        {
            var pic = (PictureBox)sender;

            // Image: Custom
            // Drag style: Like Windows Explorer

            pic.InitializeDragAndDrop()
                .Copy()
                .Immediately()
                .WithData(pic.Image)
                .WithPreview(Grayscale((Bitmap)pic.Image)).LikeWindowsExplorer()
                .To(PreviewBoxes, (target, data) => target.Image = data);
        }

        private void picUpdatingPreviewRelativeToCursor_MouseDown(object sender, MouseEventArgs e)
        {
            var pic = (PictureBox)sender;

            pic.InitializeDragAndDrop()
                .Copy()
                .Immediately()
                .WithData(pic.Image)
                .WithPreview(img => new UpdatablePreview(img, Control.MousePosition)).RelativeToCursor()
                .To(PreviewBoxes, (target, data) => target.Image = data);
        }

        private void CountryList_MouseDown(object sender, MouseEventArgs e)
        {
            var source = (ListView)sender;
            var target = source.Equals(listLeft) ? listRight : listLeft;

            source.InitializeDragAndDrop()
                .Move()
                .OnMouseMove()
                .If(() => source.SelectedItems.OfType<ListViewItem>().ToArray().Any())
                .WithData(() => source.SelectedItems.OfType<ListViewItem>().ToArray())
                .WithPreview(_ => RenderPreview(source.SelectedItems.OfType<ListViewItem>().ToArray())).BehindCursor()
                .To(target, MoveItems);
        }

        private void linkCompatibilityBrowser_MouseDown(object sender, MouseEventArgs e)
        {
            linkCompatibilityBrowser.InitializeDragAndDrop()
                .Link()
                .Immediately()
                .WithData("https://twitter.com/waescher");
        }

        private void listCompatibilityFluent_MouseDown(object sender, MouseEventArgs e)
        {
            listCompatibilityFluent.InitializeDragAndDrop()
                .Copy()
                .OnMouseMove()
                .If(() => listCompatibilityFluent.SelectedItems.OfType<ListViewItem>().ToArray().Any())
                .WithData(() => listCompatibilityFluent.SelectedItems.OfType<ListViewItem>().ToArray())
                .WithPreview(_ => RenderPreview(listCompatibilityFluent.SelectedItems.OfType<ListViewItem>().ToArray())).BehindCursor();
            //.To(listCompatibilityTarget, CopyItems) -> doubles items if used, because listCompatibilityTarget handles the drop input already
        }

        private void listCompatibilityTraditional_MouseDown(object sender, MouseEventArgs e)
        {
            _listCompatibilityTraditionalMouseDown = true;
        }

        private void listCompatibilityTraditional_MouseUp(object sender, MouseEventArgs e)
        {
            _listCompatibilityTraditionalMouseDown = false;
        }

        private void listCompatibilityTraditional_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_listCompatibilityTraditionalMouseDown)
                return;

            _listCompatibilityTraditionalMouseDown = false;

            if (listCompatibilityTraditional.SelectedItems.Count == 0)
                return;

            var newItems = listCompatibilityTraditional.SelectedItems.OfType<ListViewItem>().ToArray();
            listCompatibilityTraditional.DoDragDrop(newItems, DragDropEffects.Copy | DragDropEffects.Move);
        }

        private void listCompatibilityTarget_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void listCompatibilityTarget_DragDrop(object sender, DragEventArgs e)
        {
            var items = e.Data.GetData(typeof(ListViewItem[])) as ListViewItem[];
            if (items != null)
            {
                var newItems = items.Select(i => new ListViewItem { Text = i.Text, ImageIndex = i.ImageIndex }).ToArray();
                listCompatibilityTarget.Items.AddRange(newItems);
            }
        }

        private void picDragOverAllowDropFalse_MouseDown(object sender, MouseEventArgs e)
        {
            picDragOverAllowDropFalse.InitializeDragAndDrop()
                .Copy()
                .OnMouseMove()
                .WithData(() => picDragOverAllowDropFalse.Image)
                .To(picEmpty5, (target, data) => target.Image = data);
        }

        private void MoveItems(ListView targetListView, ListViewItem[] draggedItems)
        {
            var newItems = draggedItems.Select(i => new ListViewItem { Text = i.Text, ImageIndex = i.ImageIndex }).ToArray();
            targetListView.Items.AddRange(newItems);

            var sourceListView = draggedItems[0].ListView;

            foreach (var item in draggedItems)
                sourceListView.Items.Remove(item);
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
                    graphics.DrawImage(image, new Rectangle(0, 0, result.Width, result.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            return result;
        }

        private Bitmap RenderPreview(ListViewItem[] items)
        {
            var itemHeight = 23;
            var image = new Bitmap(120, itemHeight * items.Length);
            var borderBounds = new Rectangle(Point.Empty, image.Size);
            borderBounds.Width--;
            borderBounds.Height--;

            using (var graphics = Graphics.FromImage(image))
            {
                graphics.Clear(Color.White);
                graphics.DrawRectangle(Pens.Black, borderBounds);

                using (var format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Center;
                    format.FormatFlags = StringFormatFlags.NoWrap;
                    format.Trimming = StringTrimming.EllipsisCharacter;

                    for (int i = 0; i < items.Length; i++)
                    {
                        var itemY = itemHeight * i;

                        var itemImage = imlSmall.Images[items[i].ImageIndex];
                        var imagePadding = (itemHeight - itemImage.Height) / 2;
                        graphics.DrawImage(itemImage, new Point(imagePadding, itemY + imagePadding));

                        var textX = itemImage.Width + imagePadding;
                        var textBounds = new Rectangle(textX, itemY, image.Width - textX, itemHeight);
                        graphics.DrawString(items[i].Text, listLeft.Font, Brushes.Black, textBounds, format);
                    }
                }
            }

            return image;
        }

        private PictureBox[] PreviewBoxes => new[] { picNoPreview, picControlPreviewBehindCursor, picCustomPreviewWindowsExplorerStyle, picUpdatingPreviewRelativeToCursor, picEmpty1, picEmpty2, picEmpty3, picEmpty4 };
    }
}