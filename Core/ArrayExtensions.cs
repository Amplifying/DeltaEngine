using System.Collections.Generic;
using System.Linq;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Additional array manipluation and to string methods.
	/// </summary>
	public static class ArrayExtensions
	{
		public static bool Contains<T>(this IEnumerable<T> array, T value)
		{
			return array.Any(item => item.Equals(value));
		}

		public static string ToText(this IEnumerable<string> texts)
		{
			return string.Join(", ", texts);
		}
	}
}