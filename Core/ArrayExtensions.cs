using System;
using System.Collections.Generic;
using System.Linq;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Additional array manipluation and array to text methods.
	/// </summary>
	public static class ArrayExtensions
	{
		public static bool Contains<T>(this Array array, T value)
		{
			return array != null && array.Cast<T>().Contains(value);
		}

		public static bool Compare<T>(this IEnumerable<T> array1, IEnumerable<T> array2)
		{
			return array1 == null && array2 == null ||
				array1 != null && array2 != null && array1.SequenceEqual(array2);
		}

		public static string ToText<T>(this IEnumerable<T> texts)
		{
			return string.Join(", ", texts);
		}
	}
}