using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using DeltaEngine.Content;

namespace DeltaEngine.Entities
{
	/// <summary>
	/// Loads saved binary data object and reconstructs them based on the saved type information.
	/// </summary>
	internal static class BinaryDataLoader
	{
		internal static object TryCreateAndLoad(Type typeToCreate, BinaryReader reader)
		{
			try
			{
				return CreateAndLoadTopLevelType(typeToCreate, reader);
			}
			catch (MissingMethodException ex)
			{
				throw new MissingMethodException(
					ex.Message + "\n\n" + "When loading '" + typeToCreate +
						"' could not create default instance of '" + subTypeToCreate + "'\n", ex);
			}
		}

		private static object CreateAndLoadTopLevelType(Type typeToCreate, BinaryReader reader)
		{
			topLevelTypeToCreate = typeToCreate;
			nestingDepth = 0;
			return CreateAndLoad(typeToCreate, reader);
		}

		private static Type topLevelTypeToCreate;
		private static int nestingDepth;

		private static object CreateAndLoad(Type typeToCreate, BinaryReader reader)
		{
			subTypeToCreate = typeToCreate;
			IncreaseNestingDepth();
			object data = GetDefault(typeToCreate);
			LoadData(ref data, typeToCreate, reader);
			nestingDepth--;
			return data;
		}

		private static Type subTypeToCreate;

		private static void IncreaseNestingDepth()
		{
			//ncrunch: no coverage start
			if (nestingDepth++ > 5)
				throw new NotSupportedException("When loading '" + topLevelTypeToCreate + "'" +
					" the nesting depth got too big. " +
					"Investigate the stack trace and only save types that are actually data types!");
			//ncrunch: no coverage end
		}

		private static object GetDefault(Type type)
		{
			if (type.IsArray || type == typeof(Type) || type.Name == "RuntimeType" ||
				typeof(ContentData).IsAssignableFrom(type))
				return null;

			return type == typeof(string) ? "" : Activator.CreateInstance(type, true);
		}

		private static void LoadData(ref object data, Type type, BinaryReader reader)
		{
			if (typeof(ContentData).IsAssignableFrom(type))
			{
				var contentName = reader.ReadString();
				data = ContentLoader.Load(type, contentName);
				return;
			}

			if (type.IsEnum)
			{
				LoadPrimitiveData(ref data, type, reader);
				data = Enum.ToObject(type, data);
				return;
			}

			if (LoadPrimitiveData(ref data, type, reader))
				return;

			if (type == typeof(byte[]))
				data = LoadByteArray(reader);
			else if (type == typeof(char[]))
				data = LoadCharArray(reader);
			else if (data is IList || type.IsArray)
				data = LoadArray(data as IList, type, reader);
			else if (type.IsClass || type.IsValueType)
				LoadClassData(data, type, reader);
		}

		private static bool LoadPrimitiveData(ref object data, Type type, BinaryReader reader)
		{
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Boolean:
				data = reader.ReadBoolean();
				return true;
			case TypeCode.Byte:
				data = reader.ReadByte();
				return true;
			case TypeCode.Char:
				data = reader.ReadChar();
				return true;
			case TypeCode.Decimal:
				data = reader.ReadDecimal();
				return true;
			case TypeCode.Double:
				data = reader.ReadDouble();
				return true;
			case TypeCode.Single:
				data = reader.ReadSingle();
				return true;
			case TypeCode.Int16:
				data = reader.ReadInt16();
				return true;
			case TypeCode.Int32:
				data = reader.ReadInt32();
				return true;
			case TypeCode.Int64:
				data = reader.ReadInt64();
				return true;
			case TypeCode.String:
				data = reader.ReadString();
				return true;
			case TypeCode.SByte:
				data = reader.ReadSByte();
				return true;
			case TypeCode.UInt16:
				data = reader.ReadUInt16();
				return true;
			case TypeCode.UInt32:
				data = reader.ReadUInt32();
				return true;
			case TypeCode.UInt64:
				data = reader.ReadUInt64();
				return true;
			}
			return false;
		}

		private static byte[] LoadByteArray(BinaryReader reader)
		{
			int count = reader.ReadInt32();
			return reader.ReadBytes(count);
		}

		private static char[] LoadCharArray(BinaryReader reader)
		{
			int count = reader.ReadInt32();
			return reader.ReadChars(count);
		}

		private static object LoadArray(IList list, Type arrayType, BinaryReader reader)
		{
			int count = reader.ReadInt32();
			if (list == null)
				list = Activator.CreateInstance(arrayType, new object[] { count }) as IList;

			if (count == 0)
				return list;

			var arrayElementType = (BinaryDataSaver.ArrayElementType)reader.ReadByte();
			if (arrayElementType == BinaryDataSaver.ArrayElementType.AllTypesAreDifferent)
				LoadArrayWhereAllElementsAreNotTheSameType(list, arrayType, reader, count);
			else
			{
				Type elementType = arrayElementType == BinaryDataSaver.ArrayElementType.AllTypesAreTypes
					? typeof(Type) : reader.ReadString().GetTypeFromShortNameOrFullNameIfNotFound();
				LoadArrayWhereAllElementsAreTheSameType(list, arrayType, reader, count, elementType);
			}

			return list;
		}

		private static void LoadArrayWhereAllElementsAreTheSameType(IList list, Type arrayType,
			BinaryReader reader, int count, Type elementType)
		{
			if (!arrayType.IsArray)
				list.Clear();

			for (int i = 0; i < count; i++)
				if (arrayType.IsArray)
					list[i] = CreateAndLoad(elementType, reader);
				else
					list.Add(CreateAndLoad(elementType, reader));
		}

		private static void LoadArrayWhereAllElementsAreNotTheSameType(IList list, Type arrayType,
			BinaryReader reader, int count)
		{
			if (arrayType.IsArray)
				LoadArrayWithItsTypes(list, reader, count);
			else
				LoadListWithItsTypes(list, reader, count);
		}

		private static void LoadArrayWithItsTypes(IList list, BinaryReader reader, int count)
		{
			for (int i = 0; i < count; i++)
				LoadArrayElement(list, reader, i);
		}

		private static void LoadArrayElement(IList list, BinaryReader reader, int i)
		{
			Type elementType = reader.ReadString().GetTypeFromShortNameOrFullNameIfNotFound();
			list[i] = CreateAndLoad(elementType, reader);
		}

		private static void LoadListWithItsTypes(IList list, BinaryReader reader, int count)
		{
			list.Clear();
			for (int i = 0; i < count; i++)
				LoadListElement(list, reader);
		}

		private static void LoadListElement(IList list, BinaryReader reader)
		{
			Type elementType = reader.ReadString().GetTypeFromShortNameOrFullNameIfNotFound();
			list.Add(CreateAndLoad(elementType, reader));
		}

		private static void LoadClassData(object data, Type type, BinaryReader reader)
		{
			foreach (FieldInfo field in GetBackingFields(type))
			{
				Type fieldType = field.FieldType;
				if (field.FieldType.IsClass)
				{
					bool isNull = !reader.ReadBoolean();
					if (isNull)
						continue;

					if (fieldType.AssemblyQualifiedName != null && !field.FieldType.IsArray &&
						field.FieldType != typeof(string) && !typeof(IList).IsAssignableFrom(field.FieldType))
						fieldType = reader.ReadString().GetTypeFromShortNameOrFullNameIfNotFound();
				}
				field.SetValue(data, CreateAndLoad(fieldType, reader));
			}
		}

		internal static IEnumerable<FieldInfo> GetBackingFields(this Type type)
		{
			if (type.Name == "RuntimeType")
				return new List<FieldInfo>();

			FieldInfo[] fields =
				type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (!type.IsValueType && type.BaseType != null && type.BaseType != typeof(object))
				return CombineFields(GetBackingFields(type.BaseType), fields);

			if (!type.IsExplicitLayout)
				return fields;

			var offsets = new List<int>();
			var nonDuplicateFields = new List<FieldInfo>();
			foreach (FieldInfo field in fields)
			{
				object[] offsetAttributes = field.GetCustomAttributes(typeof(FieldOffsetAttribute), false);
				int offsetValue = (offsetAttributes[0] as FieldOffsetAttribute).Value;
				if (offsets.Contains(offsetValue))
					continue;

				nonDuplicateFields.Add(field);
				offsets.Add(offsetValue);
			}
			return nonDuplicateFields;
		}

		private static IEnumerable<FieldInfo> CombineFields(IEnumerable<FieldInfo> baseBackingFields,
			IEnumerable<FieldInfo> fields)
		{
			var combinedFields = new List<FieldInfo>(baseBackingFields);
			foreach (var field in fields)
				if (combinedFields.All(existingField => existingField.Name != field.Name))
					combinedFields.Add(field);

			return combinedFields;
		}
	}
}