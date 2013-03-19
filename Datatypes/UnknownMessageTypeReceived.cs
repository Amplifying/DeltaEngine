using System;

namespace DeltaEngine.Datatypes
{
	public class UnknownMessageTypeReceived : Exception
	{
		//ncrunch: no coverage start
		public UnknownMessageTypeReceived(string message)
			: base(message) { } 
	}
}