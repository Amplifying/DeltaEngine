namespace DeltaEngine.Networking
{
	public class UnknownMessage : TextMessage
	{
		public UnknownMessage(string error)
			: base(error) {}

		/// <summary>
		/// Need empty constructor for BinaryDataExtensions class reconstruction
		/// </summary>
		private UnknownMessage() {}
	}
}