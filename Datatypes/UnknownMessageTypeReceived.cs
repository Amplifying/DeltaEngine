using System;

namespace DeltaEngine.Datatypes
{
	public class UnknownMessageTypeReceived : Exception
	{
		public UnknownMessageTypeReceived(string message)
			: base(message) { }
	}
}