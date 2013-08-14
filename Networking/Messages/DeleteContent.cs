namespace DeltaEngine.Networking.Messages
{
	/// <summary>
	/// Deletes a content file on the server or client, whoever has obsolete files.
	/// </summary>
	public class DeleteContent : ContentMessage
	{
		private DeleteContent() {}

		public DeleteContent(string contentName)
		{
			ContentName = contentName;
		}

		public string ContentName { get; private set; }
	}
}