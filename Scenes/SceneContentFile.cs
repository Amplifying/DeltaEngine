namespace DeltaEngine.Scenes
{
	/// <summary>
	/// Implements SceneContent by loading a Scene from a file
	/// </summary>
	public sealed class SceneContentFile : SceneContent
	{
		public SceneContentFile(string contentFilename)
			: base(contentFilename)
		{
			Scene = new SceneFile("Content/" + contentFilename + ".dat").Scene;
		}
	}
}