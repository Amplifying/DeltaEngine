using DeltaEngine.Content;

namespace DeltaEngine.Graphics
{
	public abstract class Mesh : ContentData
	{
		protected Mesh(string contentName)
			: base(contentName) {}

		public abstract int NumberOfVertices { get; }
		public abstract void Draw();
	}
}