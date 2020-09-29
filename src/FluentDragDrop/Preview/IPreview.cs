using System;

namespace FluentDragDrop.Preview
{
    public interface IPreview
    {
        event EventHandler<Preview> Updated;

        void Start();

        void Stop();

        Preview Get();
    }
}