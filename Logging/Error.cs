using System;

namespace DeltaEngine.Logging
{
	/// <summary>
	/// In case of a fatal error the program will exit and this type of message is logged.
	/// </summary>
	public class Error : Warning
	{
		public Error() { }

		public Error(Exception ex)
			: base(ex) {}
	}
}