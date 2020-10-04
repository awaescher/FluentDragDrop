# Fluent Drag&Drop

[![NuGet Status](https://img.shields.io/nuget/v/FluentDragDrop.svg)](https://www.nuget.org/packages/FluentDragDrop/)

Drag&Drop in WinForms is cumbersome and error-prone. There are multiple events to handle, members to track and properties to set on at least two controls. Passing data is kind of special and you don't get preview images while dragging things around.

Wouldn't it be great if you could use Drag&Drop with fluent code like this?

<!-- snippet: ImmediateUsage -->
<a id='snippet-immediateusage'></a>
```cs
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
```
<sup><a href='/src/FluentDragDropExample/TestForm.cs#L29-L48' title='File snippet `immediateusage` was extracted from'>snippet source</a> | <a href='#snippet-immediateusage' title='Navigate to start of snippet `immediateusage`'>anchor</a></sup>
<!-- endSnippet -->

It's all in there: Putting data to the drag&drop operation, attaching a custom preview image to the mouse cursor, working with the dragged data once it's dropped and much more.

![Screenshot](doc/PreviewDragStyles.gif)

> Did you notice that you can even update preview images and their opacity at any time while dragging? 😉

## Perfect by default

In real world apps, Drag&Drop should not start at MouseDown because the control selection might not be up to date to the mouse interaction. Instead, developers typically have to keep track that MouseDown did happen and add an additional event handler to MouseMove. If MouseMove is triggered and there was a MouseDown but no MouseUp, the user pressed the mouse button and moved the mouse. So that's when he wanted to start a Drag&Drop operation.

Careful developers won't track a flag whether the mouse button is still down or not but the location where the user pressed the button. On MouseMove, they can then calculate the distance how much the cursor did move. Having a few pixels buffer here helps to prevent the user from accidential Drag&Drop operations. 

FluentDrag&Drop does exactly that if defined with `OnMouseMove()`. However there is no flag or location to track. Methods like `WithData()` won't accept direct variables to be passed anymore, instead they require a function which gets called as soon as the mouse move is detected later on - if the user pressed and held the mouse button and if he moved a few pixels. Additionally, the developer has the possibility to validate the input like a valid control selection with `If()`. Of course, this gets called just like `WithData()` as soon as the user moves the mouse accordingly:

<!-- snippet: DelayedUsage -->
<a id='snippet-delayedusage'></a>
```cs
private void CountryList_MouseDown(object sender, MouseEventArgs e)
{
    var source = (ListView)sender;
    var target = source.Equals(listLeft) ? listRight : listLeft;

    source.InitializeDragAndDrop()
        .Move()
        .OnMouseMove()
        .If(() => source.SelectedIndices.Count > 0)
        .WithData(() => source.SelectedItems.OfType<ListViewItem>().ToArray())
        .WithPreview((_, data) => RenderPreview(data)).BehindCursor()
        .To(target, MoveItems);
}
```
<sup><a href='/src/FluentDragDropExample/TestForm.cs#L77-L91' title='File snippet `delayedusage` was extracted from'>snippet source</a> | <a href='#snippet-delayedusage' title='Navigate to start of snippet `delayedusage`'>anchor</a></sup>
<!-- endSnippet -->

This (<sub><sup>and the 5 line method `MoveItems()`</sub></sup>) is everything we need to implement two-way Drag&Drop lists:

![Screenshot](doc/RealWorld.gif)

## Compatibility

FluentDrag&Drop can easily be used with your current Drag&Drop implementations if you want. The following animation shows how it works in combination with traditional Drag&Drop implementations as we know with events like `DragEnter`, `DragOver` and `DragDrop`:

![Screenshot](doc/Compatibility.gif)

## Smoothness

Most approaches I have used in the past get in trouble when moving the preview over controls that do not have the property `AllowDrop` set to `true`. Whenever a Drag&Drop implementation uses the `GiveFeedback` event to update its preview images, you'll get a behavior like this:

![Screenshot](doc/AllowDropFalseWithoutFluent.gif)

In contrast, FluentDrag&Drop will render preview images smoothly wherever you move them.

![Screenshot](doc/AllowDropFalse.gif)
