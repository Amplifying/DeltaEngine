using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Scenes
{
	/// <summary>
	/// Similar to BinaryDataFactory but Loads using Content + Input + BinaryReader
	/// </summary>
	public sealed class ControlFactory : IDisposable
	{
		public ControlFactory(Content content, Input.Input input)
		{
			this.content = content;
			this.input = input;
			mainAppDomain = AppDomain.CurrentDomain;
			RegisterAvailableBinaryDataImplementation();
			mainAppDomain.AssemblyLoad += OnAssemblyLoadInCurrentDomain;
		}

		private readonly Content content;
		private readonly Input.Input input;
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
				if (typeof(Control).IsAssignableFrom(type) && !type.IsAbstract)
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

		public void Save(Control control, BinaryWriter writer)
		{
			writer.Write(GetShortName(control));
			control.Save(writer);
		}

		internal string GetShortName(Control control)
		{
			return shortNames[control.GetType()];
		}

		public Control Load(BinaryReader reader)
		{
			string shortName = reader.ReadString();
			if (typeMap.ContainsKey(shortName))
				return CreateAndLoad(shortName, reader);

			throw new UnknownBinaryDataTypeException(shortName);
		}

		private Control CreateAndLoad(string typeShortName, BinaryReader reader)
		{
			Control control = Create(typeShortName);
			control.Load(content, input, reader);
			return control;
		}

		internal Control Create(string typeShortName)
		{
			Type foundType = typeMap[typeShortName];
			return (Control)Activator.CreateInstance(foundType, true);
		}

		public void Dispose()
		{
			mainAppDomain.AssemblyLoad -= OnAssemblyLoadInCurrentDomain;
		}
	}
}