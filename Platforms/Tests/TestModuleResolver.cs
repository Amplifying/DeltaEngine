namespace DeltaEngine.Platforms.Tests
{
	public abstract class TestModuleResolver
	{
		protected TestModuleResolver(TestResolver testResolver)
		{
			this.testResolver = testResolver;
		}

		protected TestResolver testResolver;
	}
}
