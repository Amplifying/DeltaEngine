using DeltaEngine.Content;

namespace DeltaEngine.Scenes
{
	/// <summary>
	/// Loads a Scene via the Content system
	/// </summary>
	public abstract class SceneContent : ContentData
	{
		protected SceneContent(string contentName)
			: base(contentName) { }

		public Scene Scene { get; set; }
	}
}