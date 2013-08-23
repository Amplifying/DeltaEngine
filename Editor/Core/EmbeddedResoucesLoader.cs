using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DeltaEngine.Editor.Core
{
	public static class EmbeddedResoucesLoader
	{
		public static string GetFullResourceName(string resourceName)
		{
			var callingAssembly = GetCallingAssembly();
			var resourceNameWithNamespace = BuildFullResourceName(callingAssembly, resourceName);
			if (ResourceExists(callingAssembly, resourceNameWithNamespace))
				return resourceNameWithNamespace;
			throw new EmbeddedResourceNotFound(resourceName);
		}

		private static Assembly GetCallingAssembly()
		{
			var stackTrace = new StackTrace();
			StackFrame[] stackFrames = stackTrace.GetFrames();
			foreach (var stackFrame in stackFrames)
				if (IsCallingAssembly(stackFrame))
					return stackFrame.GetMethod().DeclaringType.Assembly;
			throw new Exception("No calling assembly found"); //ncrunch: no coverage
		}

		private static bool IsCallingAssembly(StackFrame stackFrame)
		{
			var stackFrameNameSpace = stackFrame.GetMethod().DeclaringType.Assembly.GetName().Name;
			var resourceLoaderNamespace = Assembly.GetExecutingAssembly().GetName().Name;
			return stackFrameNameSpace != resourceLoaderNamespace;
		}

		private static string BuildFullResourceName(Assembly callingAssembly, string resourceName)
		{
			return callingAssembly.GetName().Name + "." + resourceName;
		}

		private static bool ResourceExists(Assembly callingAssembly, string resourceNameWithNamespace)
		{
			var manifestResourceNames = callingAssembly.GetManifestResourceNames().ToList();
			return manifestResourceNames.Contains(resourceNameWithNamespace);
		}

		internal class EmbeddedResourceNotFound : Exception
		{
			public EmbeddedResourceNotFound(string resourceName)
				: base(resourceName) {}
		}

		public static Stream GetEmbeddedResourceStream(string resourceName)
		{
			var callingAssembly = GetCallingAssembly();
			var fullResourceName = GetFullResourceName(resourceName);
			return callingAssembly.GetManifestResourceStream(fullResourceName);
		}
	}
}