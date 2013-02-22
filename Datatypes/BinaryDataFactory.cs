using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DeltaEngine.Datatypes
{
	public sealed class BinaryDataFactory : IDisposable
	{
		public BinaryDataFactory()
		{
			mainAppDomain = AppDomain.CurrentDomain;
			RegisterAvailableBinaryDataImplementation();
			mainAppDomain.AssemblyLoad += OnAssemblyLoadInCurrentDomain;
		}

		private readonly AppDomain mainAppDomain;

		private void RegisterAvailableBinaryDataImplementation()
		{
			Assembly[] assemblies = mainAppDomain.GetAssemblies();
			foreach (var assembly in assemblies)
				if (new AssemblyChecker(assembly).IsAllowed)
					AddAssemblyTypes(assembly);
		}

		private void AddAssemblyTypes(Assembly assembly)
		{
			Type[] types = assembly.GetTypes();
			foreach (var type in types)
				if (typeof(BinaryData).IsAssignableFrom(type) && !type.IsAbstract)
					AddType(type);
		}

		private void AddType(Type type)
		{
			var shortName = type.Name;
			if (typeMap.ContainsKey(shortName))
				shortName = type.FullName;

			shortNames.Add(type, shortName);
			typeMap.Add(shortName, type);
		}

		private readonly Dictionary<string, Type> typeMap = new Dictionary<string, Type>();
		private readonly Dictionary<Type, string> shortNames = new Dictionary<Type, string>();

		private void OnAssemblyLoadInCurrentDomain(object sender, AssemblyLoadEventArgs e)
		{
			AddAssemblyTypes(e.LoadedAssembly);
		}

		internal BinaryData Create(string typeShortName)
		{
			Type foundType = typeMap[typeShortName];
			return (BinaryData)Activator.CreateInstance(foundType, true);
		}

		internal string GetShortName(BinaryData data)
		{
			return shortNames[data.GetType()];
		}

		public void Save(BinaryData data, BinaryWriter writer)
		{
			writer.Write(GetShortName(data));
			data.Save(writer);
		}

		private BinaryData CreateAndLoad(string typeShortName, BinaryReader reader)
		{
			BinaryData data = Create(typeShortName);
			data.Load(reader);
			return data;
		}

		public BinaryData Load(BinaryReader reader)
		{
			string shortName = reader.ReadString();
			if (typeMap.ContainsKey(shortName))
				return CreateAndLoad(shortName, reader);

			throw new UnknownMessageTypeReceived(shortName);
		}

		public void Dispose()
		{
			mainAppDomain.AssemblyLoad -= OnAssemblyLoadInCurrentDomain;
		}
	}

	public class AssemblyChecker
	{
		public AssemblyChecker(Assembly assembly)
		{
			assemblyName = assembly.GetName().Name;
		}

		private readonly string assemblyName;

		public bool IsAllowed
		{
			get { return !(IsMicrosoftAssembly() || IsIdeHelperTool() || IsThirdPartyLibrary()); }
		}

		protected bool IsMicrosoftAssembly()
		{
			return StartsWith("System", "mscorlib", "WindowsBase", "PresentationFramework",
				"PresentationCore", "WindowsFormsIntegration", "Microsoft.", "Roslyn.");
		}

		private bool StartsWith(params string[] partialNames)
		{
			return
				partialNames.Any(
					x => assemblyName.StartsWith(x, StringComparison.InvariantCultureIgnoreCase));
		}

		private bool IsIdeHelperTool()
		{
			return StartsWith("JetBrains.", "NUnit.Core.", "NCrunch.", "ReSharper.");
		}

		private bool IsThirdPartyLibrary()
		{
			return StartsWith("OpenAL32", "wrap_oal", "libEGL", "libgles", "libGLESv2", "libvlc",
				"libvlccore", "csogg", "csvorbis");
		}
	}
}