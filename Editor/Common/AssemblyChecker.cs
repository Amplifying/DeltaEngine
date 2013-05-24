using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DeltaEngine.Core;

namespace DeltaEngine.Editor.Common
{
	public class AssemblyChecker
	{
		public AssemblyChecker()
		{
			OutdatedAssemblies = new List<Assembly>();
		}

		public List<Assembly> OutdatedAssemblies { get; private set; }

		public void CheckCurrentAppDomain()
		{
			var assemblies = new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
			foreach (var assembly in assemblies)
				if (assembly.IsAllowed() && IsAssemblyOutOfDate(assembly))
					OutdatedAssemblies.Add(assembly);
		}

		private static bool IsAssemblyOutOfDate(Assembly assembly)
		{
			return (DateTime.Now - File.GetLastWriteTime(assembly.Location)).TotalDays > MaxAgeInDays;
		}

		private const int MaxAgeInDays = 7;
	}
}