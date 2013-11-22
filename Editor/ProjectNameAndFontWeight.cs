using System;
using System.Windows;

namespace DeltaEngine.Editor
{
	public class ProjectNameAndFontWeight : IEquatable<ProjectNameAndFontWeight>
	{
		public ProjectNameAndFontWeight()
		{
			ResetToDefault();
		}

		public void ResetToDefault()
		{
			Name = DefaultName;
			Weight = FontWeights.Normal;
		}

		public const string DefaultName = "GhostWars";
		public string Name { get; private set; }
		public FontWeight Weight { get; private set; }

		public ProjectNameAndFontWeight(string name, FontWeight weight)
		{
			Name = name;
			Weight = weight;
		}

		public bool IsDefault()
		{
			return Name == DefaultName;
		}

		public bool Equals(ProjectNameAndFontWeight other)
		{
			return Name == other.Name && Weight == other.Weight;
		}
	}
}