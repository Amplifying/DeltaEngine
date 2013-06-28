using System;
using System.Linq;

namespace DeltaEngine.Platforms
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class IgnoreForResolverAttribute : Attribute
	{
		public static bool IsTypeIgnored(Type t)
		{
			object[] attributes = t.GetCustomAttributes(false);
			return attributes.OfType<IgnoreForResolverAttribute>().Any();
		}
	}
}