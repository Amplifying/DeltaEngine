using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DeltaEngine.Core;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Allows to easily save and recreate BinaryData objects with the full type names.
	/// Make sure to load all assemblies before reconstructing any type.
	/// </summary>
	public static class BinaryDataExtension
	{
		static BinaryDataExtension()
		{
			RegisterAvailableBinaryDataImplementation();
			AppDomain.CurrentDomain.AssemblyLoad += (o, args) => AddAssemblyTypes(args.LoadedAssembly);
		}
		
		private static void RegisterAvailableBinaryDataImplementation()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies)
				if (new AssemblyChecker(assembly).IsAllowed)
					AddAssemblyTypes(assembly);
		}

		private static void AddAssemblyTypes(Assembly assembly)
		{
			Type[] types = assembly.GetTypes();
			foreach (var type in types)
				if (typeof(BinaryData).IsAssignableFrom(type) && !type.IsAbstract)
					AddType(type);
		}

		private static void AddType(Type type)
		{
			var shortName = type.Name;
			if (TypeMap.ContainsKey(shortName))
				shortName = type.FullName;

			ShortNames.Add(type, shortName);
			TypeMap.Add(shortName, type);
		}

		private static readonly Dictionary<string, Type> TypeMap = new Dictionary<string, Type>();
		private static readonly Dictionary<Type, string> ShortNames = new Dictionary<Type, string>();

		internal static string GetShortName(this BinaryData data)
		{
			return ShortNames[data.GetType()];
		}

		/// <summary>
		/// Saves any BinaryData object type information and the actual data contained in in.
		/// </summary>
		public static void Save(this BinaryData data, BinaryWriter writer)
		{
			writer.Write(GetShortName(data));
			data.SaveData(writer);
		}

		/// <summary>
		/// Loads a BinaryData object and reconstructs the object based on the saved type information.
		/// </summary>
		public static DataObject Create<DataObject>(this BinaryReader reader)
			where DataObject : class, BinaryData
		{
			string shortName = reader.ReadString();
			if (TypeMap.ContainsKey(shortName))
				return CreateAndLoad(TypeMap[shortName], reader) as DataObject;

			throw new UnknownMessageTypeReceived(shortName);
		}

		private static BinaryData CreateAndLoad(Type typeToCreate, BinaryReader reader)
		{
			try
			{
				return TryCreateAndLoad(typeToCreate, reader);
			}
			catch (MissingMethodException ex)
			{
				throw new MissingMethodException(ex.Message + " " + typeToCreate);
			}
		}

		private static BinaryData TryCreateAndLoad(Type typeToCreate, BinaryReader reader)
		{
			var data = (BinaryData)Activator.CreateInstance(typeToCreate, true);
			data.LoadData(reader);
			return data;
		}

		public class UnknownMessageTypeReceived : Exception
		{
			public UnknownMessageTypeReceived(string message)
				: base(message) { }
		}

		public static MemoryStream SaveToMemoryStream(this BinaryData binaryData)
		{
			var data = new MemoryStream();
			var writer = new BinaryWriter(data);
			Save(binaryData, writer);
			return data;
		}

		public static DataObject CreateFromMemoryStream<DataObject>(this MemoryStream data)
			where DataObject : class, BinaryData
		{
			data.Seek(0, SeekOrigin.Begin);
			return Create<DataObject>(new BinaryReader(data));
		}

		public static byte[] ToByteArrayWithLengthHeader(this BinaryData message)
		{
			byte[] data = message.ToByteArray();
			byte[] head = BitConverter.GetBytes(data.Length);
			var total = new byte[data.Length + head.Length];
			head.CopyTo(total, 0);
			data.CopyTo(total, head.Length);
			return total;
		}

		public static byte[] ToByteArray(this BinaryData message)
		{
			using (var messageStream = new MemoryStream())
			using (var messageWriter = new BinaryWriter(messageStream))
			{
				Save(message, messageWriter);
				return messageStream.ToArray();
			}
		}

		public static DataObject ToBinaryData<DataObject>(this byte[] data)
			where DataObject : class, BinaryData
		{
			using (var messageStream = new MemoryStream(data))
			using (var messageReader = new BinaryReader(messageStream))
				return Create<DataObject>(messageReader);
		}
	}
}