using System;

namespace DeltaEngine.Logging
{
	/// <summary>
	/// If something bad happened but we can continue this type of message is logged.
	/// </summary>
	public class Warning : Info
	{
		public Warning() {}

		public Warning(string text)
			: base(text) {}

		public Warning(Exception ex)
			: base(ex.ToString()) {}
	}
}