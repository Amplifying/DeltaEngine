using System;
using System.Linq;
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

			return StackTraceExtensions.GetEntryName();
		}

		public static bool IsAllowed(this AssemblyName assembly)
		{
			assemblyName = assembly.Name;
			return !(IsMicrosoftAssembly() || IsIdeHelperTool() || IsThirdPartyLibrary());
		}
		
		public static bool IsAllowed(this Assembly assembly)
		{
			assemblyName = assembly.GetName().Name;
			return !(IsMicrosoftAssembly() || IsIdeHelperTool() || IsThirdPartyLibrary());
		}
		
		private static string assemblyName;

		private static bool IsMicrosoftAssembly()
		{
			return StartsWith("System", "mscorlib", "WindowsBase", "PresentationFramework",
				"PresentationCore", "WindowsFormsIntegration", "Microsoft.");
		}

		private static bool StartsWith(params string[] partialNames)
		{
			return
				partialNames.Any(
					x => assemblyName.StartsWith(x, StringComparison.InvariantCultureIgnoreCase));
		}

		private static bool IsIdeHelperTool()
		{
			return StartsWith("JetBrains.", "NUnit.", "NCrunch.", "ReSharper.");
		}

		private static bool IsThirdPartyLibrary()
		{
			return StartsWith("OpenAL32", "wrap_oal", "libEGL", "libgles", "libGLESv2", "libvlc",
				"libvlccore", "csogg", "csvorbis", "Autofac", "Moq", "DynamicProxyGen",
				"Anonymously Hosted", "AvalonDock");
		}
	}
}