namespace DeltaEngine.Platforms.Tests.ModuleMocks
{
	public abstract class BaseMocks
	{
		internal BaseMocks(AutofacStarterForMockResolver resolver)
		{
			this.resolver = resolver;
		}

		internal readonly AutofacStarterForMockResolver resolver;
	}
}
