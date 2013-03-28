using DeltaEngine.Core;

namespace DeltaEngine.Scenes
{
	/// <summary>
	/// Loads a Scene via the Content system
	/// </summary>
	public abstract class SceneContent : ContentData
	{
		protected SceneContent(string contentFilename)
			: base(contentFilename) { }

		public virtual Scene Scene { get; set; }

		public override void Dispose() { }
	}
}