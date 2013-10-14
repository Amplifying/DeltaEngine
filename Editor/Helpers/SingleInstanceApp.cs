using System.Collections.Generic;

namespace DeltaEngine.Editor.Helpers
{
	public interface SingleInstanceApp
	{
		bool SignalExternalCommandLineArguments(IList<string> args);
	}
}