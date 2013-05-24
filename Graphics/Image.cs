using DeltaEngine.Content;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Graphics
{
	/// <summary>
	/// Provides a way to load images. Use Drawing to show them on the screen.
	/// </summary>
	public abstract class Image : ContentData
	{
		protected Image(string contentName)
			: base(contentName) {}

		public abstract Size PixelSize { get; }
		public bool DisableLinearFiltering { get; set; }
		public abstract bool HasAlpha { get; }

		protected readonly Color[] checkerMapColors =
		{
			Color.LightGray, Color.DarkGray,
			Color.LightGray, Color.DarkGray, Color.DarkGray, Color.LightGray, Color.DarkGray,
			Color.LightGray, Color.LightGray, Color.DarkGray, Color.LightGray, Color.DarkGray,
			Color.DarkGray, Color.LightGray, Color.DarkGray, Color.LightGray
		};

		protected static readonly Size DefaultTextureSize = new Size(4, 4);
	}
}