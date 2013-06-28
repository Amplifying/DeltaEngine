using System;
using System.Collections;
using System.IO;
using System.Reflection;
using DeltaEngine.Content;

namespace DeltaEngine.Entities
{
	/// <summary>
	/// Easily save data objects with the full type names like other Serializers, but much faster.
	/// </summary>
	internal static class BinaryDataSaver
	{
		internal static void TrySaveData(object data, Type type, BinaryWriter writer)
		{
			try
			{
				SaveData(data, type, writer);
			}
			catch (Exception ex)
			{
				throw new NotSupportedException("Unable to save " + data, ex);
			}
		}

		private static void SaveData(object data, Type type, BinaryWriter writer)
		{
			if (data is ContentData)
			{
				writer.Write((data as ContentData).Name);
				return;
			}

			if (type.Name.StartsWith("Xml"))
				throw new NotSupportedException("Please don't save xml data this way: " + data);

			if (SaveIfIsPrimitiveData(data, type, writer))
				return;

			if (type == typeof(byte[]))
				SaveByteArray(data, writer);
			else if (type == typeof(char[]))
				SaveCharArray(data, writer);
			else if (data is IList || type.IsArray)
				SaveArray(data as IList, writer);
			else if (type.IsClass || type.IsValueType)
				SaveClassData(data, type, writer);
		}

		private static bool SaveIfIsPrimitiveData(object data, Type type, BinaryWriter writer)
		{
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Boolean:
				writer.Write((bool)data);
				return true;
			case TypeCode.Byte:
				writer.Write((byte)data);
				return true;
			case TypeCode.Char:
				writer.Write((char)data);
				return true;
			case TypeCode.Decimal:
				writer.Write((decimal)data);
				return true;
			case TypeCode.Double:
				writer.Write((double)data);
				return true;
			case TypeCode.Single:
				writer.Write((float)data);
				return true;
			case TypeCode.Int16:
				writer.Write((short)data);
				return true;
			case TypeCode.Int32:
				writer.Write((int)data);
				return true;
			case TypeCode.Int64:
				writer.Write((long)data);
				return true;
			case TypeCode.String:
				writer.Write((string)data);
				return true;
			case TypeCode.SByte:
				writer.Write((sbyte)data);
				return true;
			case TypeCode.UInt16:
				writer.Write((ushort)data);
				return true;
			case TypeCode.UInt32:
				writer.Write((uint)data);
				return true;
			case TypeCode.UInt64:
				writer.Write((ulong)data);
				return true;
			}
			return false;
		}

		private static void SaveByteArray(object data, BinaryWriter writer)
		{
			writer.Write(((byte[])data).Length);
			writer.Write((byte[])data);
		}

		private static void SaveCharArray(object data, BinaryWriter writer)
		{
			writer.Write(((char[])data).Length);
			writer.Write((char[])data);
		}

		private static void SaveArray(IList list, BinaryWriter writer)
		{
			writer.Write(list.Count);
			if (list.Count == 0)
				return;

			if (AreAllElementsTheSameType(list))
				SaveArrayWhenAllElementsAreTheSameType(list, writer);
			else
				SaveArrayWhenAllElementsAreNotTheSameType(list, writer);
		}

		private static bool AreAllElementsTheSameType(IList list)
		{
			var firstType = GetType(list[0]);
			foreach (object element in list)
				if (GetType(element) != firstType)
					return false;

			return true;
		}

		private static Type GetType(Object element)
		{
			return element == null ? typeof(object) : element.GetType();
		}

		private static void SaveArrayWhenAllElementsAreTheSameType(IList list, BinaryWriter writer)
		{
			var arrayType = list[0] != null && list[0].GetType().Name == "RuntimeType"
				? ArrayElementType.AllTypesAreTypes : ArrayElementType.AllTypesAreTheSame;
			writer.Write((byte)arrayType);
			if (arrayType == ArrayElementType.AllTypesAreTheSame)
				writer.Write(list[0].GetShortNameOrFullNameIfNotFound());
			foreach (object value in list)
				TrySaveData(value, list[0] != null ? list[0].GetType() : typeof(object), writer);
		}

		private static void SaveArrayWhenAllElementsAreNotTheSameType(IEnumerable list,
			BinaryWriter writer)
		{
			writer.Write((byte)ArrayElementType.AllTypesAreDifferent);
			foreach (object value in list)
				SaveElementWithItsType(writer, value);
		}

		public enum ArrayElementType : byte
		{
			AllTypesAreDifferent,
			AllTypesAreTypes,
			AllTypesAreTheSame
		}

		private static void SaveElementWithItsType(BinaryWriter writer, object value)
		{
			if (value == null)
			{
				TrySaveData(null, typeof(object), writer);
				return;
			}

			Type type = value.GetType();
			writer.Write(value.GetShortNameOrFullNameIfNotFound());
			TrySaveData(value, type, writer);
		}

		private static void SaveClassData(object data, Type type, BinaryWriter writer)
		{
			foreach (FieldInfo field in type.GetBackingFields())
			{
				object fieldData = field.GetValue(data);
				Type fieldType = field.FieldType;
				if (field.FieldType.IsClass)
				{
					writer.Write(fieldData != null);
					if (fieldData == null)
						continue;

					fieldType = fieldData.GetType();
					if (fieldType.AssemblyQualifiedName != null && fieldType != typeof(string) &&
						!fieldType.IsArray && !typeof(IList).IsAssignableFrom(fieldType))
						writer.Write(fieldData.GetShortNameOrFullNameIfNotFound());
				}
				TrySaveData(fieldData, fieldType, writer);
			}
		}
	}
}