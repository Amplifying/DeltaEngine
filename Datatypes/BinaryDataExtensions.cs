using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DeltaEngine.Content;
using DeltaEngine.Core;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Allows to easily save and recreate binary data objects with the full type names like other
	/// Serializers, but way faster (100x). Before reconstructing types load all needed assemblies.
	/// </summary>
	public static class BinaryDataExtensions
	{
		static BinaryDataExtensions()
		{
			RegisterAvailableBinaryDataImplementation();
			AppDomain.CurrentDomain.AssemblyLoad += (o, args) => AddAssemblyTypes(args.LoadedAssembly);
		}

		private static void RegisterAvailableBinaryDataImplementation()
		{
			AddPrimitiveTypes();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies.Where(assembly => assembly.IsAllowed()))
				AddAssemblyTypes(assembly);
		}

		private static void AddPrimitiveTypes()
		{
			AddType(typeof(object));
			AddType(typeof(bool));
			AddType(typeof(byte));
			AddType(typeof(char));
			AddType(typeof(decimal));
			AddType(typeof(double));
			AddType(typeof(float));
			AddType(typeof(short));
			AddType(typeof(int));
			AddType(typeof(long));
			AddType(typeof(string));
			AddType(typeof(sbyte));
			AddType(typeof(ushort));
			AddType(typeof(uint));
			AddType(typeof(ulong));
		}

		private static void AddAssemblyTypes(Assembly assembly)
		{
			Type[] types = assembly.GetTypes();
			foreach (Type type in types.Where(type => IsValidBinaryDataType(type)))
				AddType(type);
		}

		private static bool IsValidBinaryDataType(Type type)
		{
			if (type.IsAbstract || typeof(Exception).IsAssignableFrom(type))
				return false;

			string name = type.FullName;
			return !name.StartsWith("NUnit.") && !name.EndsWith("Tests") && !name.Contains("<");
		}

		private static void AddType(Type type)
		{
			string shortName = type.Name;
			if (TypeMap.ContainsKey(shortName))
			{
				shortName = type.FullName;
				if (TypeMap.ContainsKey(shortName))
					return;
			}

			ShortNames.Add(type, shortName);
			TypeMap.Add(shortName, type);
		}

		private static readonly Dictionary<string, Type> TypeMap = new Dictionary<string, Type>();
		private static readonly Dictionary<Type, string> ShortNames = new Dictionary<Type, string>();

		internal static string GetShortName(this object data)
		{
			if (ShortNames.ContainsKey(data.GetType()))
				return ShortNames[data.GetType()];

			throw new KeyNotFoundException("There is no ShortName stored for " + data + " [" +
				data.GetType() + "]");
		}

		internal static string GetShortNameOrFullNameIfNotFound(this object data)
		{
			var type = data == null ? typeof(object) : data.GetType();
			return ShortNames.ContainsKey(type) ? ShortNames[type] : type.AssemblyQualifiedName;
		}

		internal static Type GetTypeFromShortNameOrFullNameIfNotFound(this string typeName)
		{
			if (TypeMap.ContainsKey(typeName))
				return TypeMap[typeName];
			else
				return Type.GetType(typeName);
		}

		/// <summary>
		/// Saves any object type information and the actual data contained in in, use Create to load.
		/// </summary>
		public static void Save(this object data, BinaryWriter writer)
		{
			writer.Write(data.GetShortName());
			BinaryDataSaver.TrySaveData(data, data.GetType(), writer);
		}

		/// <summary>
		/// Loads a binary data object and reconstructs the object based on the saved type information.
		/// </summary>
		public static object Create(this BinaryReader reader,
			ContentLoader optionalContentLoaderToLoadContentData = null)
		{
			string shortName = reader.ReadString();
			if (TypeMap.ContainsKey(shortName))
				return BinaryDataLoader.TryCreateAndLoad(TypeMap[shortName], reader,
					optionalContentLoaderToLoadContentData);

			throw new UnknownMessageTypeReceived(shortName);
		}

		public class UnknownMessageTypeReceived : Exception
		{
			public UnknownMessageTypeReceived(string message)
				: base(message) {}
		}

		public static MemoryStream SaveToMemoryStream(this object binaryData)
		{
			var data = new MemoryStream();
			var writer = new BinaryWriter(data);
			Save(binaryData, writer);
			return data;
		}

		public static object CreateFromMemoryStream(this MemoryStream data)
		{
			data.Seek(0, SeekOrigin.Begin);
			return Create(new BinaryReader(data));
		}

		public static byte[] ToByteArrayWithLengthHeader(this object message)
		{
			byte[] data = message.ToByteArrayWithTypeInformation();
			byte[] head = BitConverter.GetBytes(data.Length);
			var total = new byte[data.Length + head.Length];
			head.CopyTo(total, 0);
			data.CopyTo(total, head.Length);
			return total;
		}

		public static byte[] ToByteArrayWithTypeInformation(this object data)
		{
			using (var messageStream = new MemoryStream())
			using (var messageWriter = new BinaryWriter(messageStream))
			{
				Save(data, messageWriter);
				return messageStream.ToArray();
			}
		}

		public static byte[] ToByteArray(this object data)
		{
			using (var messageStream = new MemoryStream())
			using (var messageWriter = new BinaryWriter(messageStream))
			{
				BinaryDataSaver.TrySaveData(data, data.GetType(), messageWriter);
				return messageStream.ToArray();
			}
		}

		public static byte[] GetBytesFromArray<Datatype>(IEnumerable<Datatype> array)
		{
			using (var messageStream = new MemoryStream())
			using (var messageWriter = new BinaryWriter(messageStream))
			{
				foreach (Datatype data in array)
					BinaryDataSaver.TrySaveData(data, data.GetType(), messageWriter);
				return messageStream.ToArray();
			}
		}

		public static object ToBinaryData(this byte[] data)
		{
			using (var messageStream = new MemoryStream(data))
			using (var messageReader = new BinaryReader(messageStream))
				return Create(messageReader);
		}
	}
}