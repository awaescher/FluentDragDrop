using System;

namespace FluentDragDrop
{
	public interface IPreview
	{
		event EventHandler<Preview> Updated;

		void Start();

		void Stop();

		Preview Get();
	}
}