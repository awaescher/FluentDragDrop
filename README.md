# Fluent Drag&Drop

Drag&Drop in WinForms is cumbersome and error-prone. There are multiple events to handle, members to track and properties to set on at least two controls.
Passing data is kind of special and you don't get preview images while dragging things aroud.

Wouldn't it be great if you could use Drag&Drop with fluent code like this?

```
private void pic1_MouseDown(object sender, MouseEventArgs e)
{
    pic1.InitializeDragAndDrop()
        .Copy()                                                   // Copy(), Move() or Link() to define allowed effects
        .Immediately()                                            // or OnMouseMove() for deferred start on mouse move
        .WithData(pic1.Image)                                     // pass any object you like
        .WithPreview(img => Watermark(img)).RelativeToCursor()    // define your preview and how it should behave
        .To(pic2, (target, data) => target.Image = data);         // use your data after it was dropped (with type safety)

}

```
It's all in there: Putting data to the drag&drop operation, attaching a custom preview image to the mouse cursor, working with the dragged data once it's dropped and much more.

![Screenshot](doc/PreviewDragStyles.gif)

> Did you notice in the 4th sample that you can even update preview images and their opacity at any time while dragging? 😉
 
 ### Compatibility
 
FluentDrag&Drop can easily be used with your current Drag&Drop implementations if you want. The following animation shows how it works in combination with traditional Drag&Drop implementations as we know with events like `DragEnter`, `DragOver` and `DragDrop`:

![Screenshot](doc/Compatibility.gif)

 ### Smoothness
 
Most approaches I have used in the past get in trouble when moving the preview over controls that do not have the property `AllowDrop` set to `true`. Whenever a Drag&Drop implementation uses the `GiveFeedback` event to update its preview images, you'll get a behavior like this:

![Screenshot](doc/AllowDropFalseWithoutFluent.gif)

In contrast, FluentDrag&Drop will render preview images smoothly whereever you move them. 

![Screenshot](doc/AllowDropFalse.gif)
