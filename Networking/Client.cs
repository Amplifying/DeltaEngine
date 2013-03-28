namespace DeltaEngine.Networking
{
	/// <summary>
	/// Provides the client side of send and receive functionality of BinaryData objects.
	/// </summary>
	public interface Client : ClientConnection
	{
		void Connect();
		void Disconnect();
	}
}