using NUnit.Framework;

namespace DeltaEngine.Datatypes.Tests
{
	public class ComponentTests
	{
		[Test]
		public void Create()
		{
			var component = new Component();
			Assert.IsNotNull(component);
		}
	}
}
