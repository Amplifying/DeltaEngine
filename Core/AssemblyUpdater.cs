using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Helps to create all Runners and Presenters in extra app domains so we can easily reload them
	/// every time they are recompiled (happens automatically with .NET Demon while typing in VS).
	/// Only used for Apps and visual tests, not for automated tests or NCrunch (slow and unsupported)
	/// </summary>
	public class AssemblyUpdater
	{
		//ncrunch: no coverage start
		public AssemblyUpdater(Action domainInitializationCode, Func<Type, object> resolver)
		{
			this.domainInitializationCode = domainInitializationCode;
			this.resolver = resolver;
			CopyAllAssembliesIntoTheAssemblyUpdaterDirectory();
			LoadAllRunnersIntoAppDomain();
		}

		private readonly Action domainInitializationCode;
		private readonly Func<Type, object> resolver;

		private void LoadAllRunnersIntoAppDomain()
		{
			domain = AppDomain.CreateDomain("Delta Engine Assembly Updater", null, CreateDomainSetup());
			domain.SetData("resolver", resolver);
			domain.SetData("domainInitializationCode", domainInitializationCode);
			domain.DoCallBack(InitializeInDomain);

			foreach (string assembly in AssemblyFilenames)
				LoadRunners(assembly);
		}

		private static void InitializeInDomain()
		{
			var initCode = AppDomain.CurrentDomain.GetData("domainInitializationCode") as Action;
			Debug.Assert(initCode != null);
			initCode.Invoke();
		}

		private AppDomain domain;

		private static void CopyAllAssembliesIntoTheAssemblyUpdaterDirectory()
		{
			if (!Directory.Exists(TargetDirectory))
				Directory.CreateDirectory(TargetDirectory);

			CopyFiles("*.dll");
			CopyFiles("*.exe");
		}

		private static string TargetDirectory
		{
			get { return Path.Combine(Directory.GetCurrentDirectory(), "AssemblyUpdater"); }
		}

		private static void CopyFiles(string filter)
		{
			string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), filter);
			foreach (string file in files)
				CopyFile(file, Path.GetFileName(file));
		}

		private static void CopyFile(string file, string filename)
		{
			Debug.Assert(filename != null);
			if (!filename.Contains("Moq.") && !filename.Contains("nunit.framework."))
				AssemblyFilenames.Add(filename);
			File.Copy(file, Path.Combine(TargetDirectory, filename), true);
		}

		private static readonly List<string> AssemblyFilenames = new List<string>();

		private static AppDomainSetup CreateDomainSetup()
		{
			return new AppDomainSetup
			{
				ApplicationBase = TargetDirectory,
				ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
			};
		}

		private void LoadRunners(string assemblyNameToLoad)
		{
			domain.SetData("assemblyNameToLoad", assemblyNameToLoad);
			domain.DoCallBack(LoadRunnersInDomain);
		}

		private static void LoadRunnersInDomain()
		{
			string oldDirectory = Directory.GetCurrentDirectory();
			Directory.SetCurrentDirectory(TargetDirectory);

			AppDomain currentDomain = AppDomain.CurrentDomain;
			var assemblyNameToLoad = currentDomain.GetData("assemblyNameToLoad") as string;
			var resolver = currentDomain.GetData("resolver") as Func<Type, object>;
			Debug.Assert(assemblyNameToLoad != null && resolver != null);

			Assembly assembly = Assembly.LoadFrom(assemblyNameToLoad);
			List<Runner> runners = currentDomain.GetData("runners") as List<Runner> ?? new List<Runner>();
			runners.AddRange(CreateRunners(assembly, resolver));
			currentDomain.SetData("runners", runners);

			Directory.SetCurrentDirectory(oldDirectory);
		}

		private static IEnumerable<Runner> CreateRunners(Assembly assembly,
			Func<Type, object> resolver)
		{
			var runners = new List<Runner>();
			foreach (Type type in assembly.GetTypes())
				if (IsNonAbstractRunner(type))
					runners.Add(resolver(type) as Runner);
			return runners;
		}

		private static bool IsNonAbstractRunner(Type t)
		{
			return !t.IsAbstract && !t.IsInterface && typeof(Runner).IsAssignableFrom(t);
		}

		public void Run()
		{
			CheckForAssemblyUpdatesEvery100Ms();
			domain.DoCallBack(RunInDomain);
		}

		private void CheckForAssemblyUpdatesEvery100Ms()
		{
			if ((DateTime.Now - lastTimeCheckedForUpdates).TotalMilliseconds < 100)
				return;

			lastTimeCheckedForUpdates = DateTime.Now;
			List<string> changedAssemblies = AssemblyFilenames.Where(HasAssemblyChanged).ToList();
			if (changedAssemblies.Count == 0)
				return;

			RestartAppDomain(changedAssemblies);
		}

		private DateTime lastTimeCheckedForUpdates = DateTime.Now;

		private static bool HasAssemblyChanged(string name)
		{
			return File.GetLastWriteTime(NormalPath(name)) > File.GetLastWriteTime(TargetPath(name));
		}

		private static string TargetPath(string assemblyFilename)
		{
			return Path.Combine(TargetDirectory, assemblyFilename);
		}

		private static string NormalPath(string assemblyFilename)
		{
			return Path.Combine(Directory.GetCurrentDirectory(), assemblyFilename);
		}

		private void RestartAppDomain(IEnumerable<string> changedAssemblies)
		{
			AppDomain.Unload(domain);

			foreach (string assembly in changedAssemblies)
				File.Copy(NormalPath(assembly), TargetPath(assembly), true);

			LoadAllRunnersIntoAppDomain();
		}

		private static void RunInDomain()
		{
			var runners = AppDomain.CurrentDomain.GetData("runners") as List<Runner>;
			Debug.Assert(runners != null);

			foreach (Runner runner in runners)
				runner.Run();
		}
	}
}