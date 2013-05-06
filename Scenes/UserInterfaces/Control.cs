using System;

namespace DeltaEngine.Scenes.UserInterfaces
{
	/// <summary>
	/// Base for all UI controls
	/// </summary>
	[Obsolete("TODO: This could use the entity system directly and be content as well!")]
	public abstract class Control : IDisposable
	{
		public string Name { get; set; }

		[Obsolete("TODO: use Visibility directly, no extra methods needed")]
		internal abstract void Show();
		[Obsolete("TODO: use Visibility directly, no extra methods needed")]
		internal abstract void Hide();
		public abstract void Dispose();
	}
}