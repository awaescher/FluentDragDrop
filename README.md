# Fluent Drag&Drop

Drag&Drop in WinForms is cumbersome and error-prone. There are multiple events to handle and properties to set on at least two controls. Passing data is ... interesting ... and you don't get preview images while dragging things aroud.

Wouldn't it be great if you could use Drag&Drop with fluent code like this?

```
private void pic1_MouseDown(object sender, MouseEventArgs e)
{
    pic1.StartDragAndDrop()
        .WithData(pic1.Image)
        .WithPreview().RelativeToCursor()
        .To(pic2, (target, data) => target.Image = data)
        .Copy();
}
```
It's all in there: Putting data to the drag&drop operation, attaching a preview image to the mouse cursor, working with the dragged data once it's dropped and more.
