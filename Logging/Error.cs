using System;

namespace DeltaEngine.Logging
{
	/// <summary>
	/// In case of a fatal error the program will exit and this type of message is logged.
	/// </summary>
	public class Error : Warning
	{
		/// <summary>
		/// Need empty constructor for BinaryDataExtensions class reconstruction
		/// </summary>
		private Error() { } 

		public Error(Exception ex)
			: base(ex) {}
	}
}