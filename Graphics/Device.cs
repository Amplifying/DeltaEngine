using System;
using DeltaEngine.Core;

namespace DeltaEngine.Graphics
{
	/// <summary>
	/// The graphics device clears everything (via Run) at the beginning of each frame and shows the
	/// result of the render buffer on screen at the end of each frame (via Present).
	/// </summary>
	public interface Device : PriorityRunner, Presenter, IDisposable {}
}