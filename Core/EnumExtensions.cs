using System;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Allows to get the number of elements in an enum.
	/// </summary>
	public static class EnumExtensions
	{
		public static Array GetEnumValues(this Enum anyEnum)
		{
			Type enumType = anyEnum.GetType();
			return Enum.GetValues(enumType);
		}

		public static int GetCount(this Enum anyEnum)
		{
			return GetEnumValues(anyEnum).Length;
		}
	}
}