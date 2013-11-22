using System.Windows;
using NUnit.Framework;

namespace DeltaEngine.Editor.Tests
{
	public class ProjectNameAndFontWeightTests
	{
		[Test]
		public void DefaultContentProjectIsGhostWars()
		{
			var expected = new ProjectNameAndFontWeight("GhostWars", FontWeights.Normal);
			Assert.AreEqual(expected.Name, ProjectNameAndFontWeight.DefaultName);
			Assert.AreEqual(expected, new ProjectNameAndFontWeight());
			var different = new ProjectNameAndFontWeight("EmptyApp", FontWeights.Bold);
			Assert.AreNotEqual(expected, different);
			Assert.IsFalse(different.IsDefault());
			different.ResetToDefault();
			Assert.AreEqual(expected, different);
			Assert.IsTrue(different.IsDefault());
		}
	}
}