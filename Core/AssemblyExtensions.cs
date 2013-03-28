using System.Reflection;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Additional methods for assembly related actions
	/// </summary>
	public static class AssemblyExtensions
	{
		//ncrunch: no coverage start
		public static string DetermineProjectName()
		{
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			if (entryAssembly != null)
				return entryAssembly.GetName().Name;

			return Assembly.GetCallingAssembly().GetName().Name + ".Tests";
		}
	}
}