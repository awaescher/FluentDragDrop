using System;
using System.Drawing;

namespace FluentDragDrop
{
	public interface IPreview
	{
		event EventHandler Updated;

		void Start();

		void Stop();

		Bitmap Get();
	}
}