using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Platforms.All
{
	/// <summary>
	/// AssemblyChecker.IsAllowed is used whenever we have to check all loaded assemblies for types.
	/// Examples include BinaryDataExtensions and AutofacResolver.RegisterAllTypesFromAllAssemblies
	/// </summary>
	public class AssemblyCheckerTests : TestWithAllFrameworks
	{
		[Test, Category("Slow")]
		public void MakeSureToOnlyIncludeAllowedDeltaEngineAndUserAssemblies()
		{
			CreateEmptyGameLoopToLoadAllAssemblies();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var assembliesAllowed = new List<string>();
			foreach (Assembly assembly in assemblies.Where(assembly => assembly.IsAllowed()))
				assembliesAllowed.Add(assembly.GetName().Name);

			Assert.AreEqual(20, assembliesAllowed.Count, "Assemblies: " + assembliesAllowed.ToText());
		}

		private void CreateEmptyGameLoopToLoadAllAssemblies()
		{
			Start(typeof(MockResolver), (ContentLoader content) => { }, () => { });
		}
	}
}