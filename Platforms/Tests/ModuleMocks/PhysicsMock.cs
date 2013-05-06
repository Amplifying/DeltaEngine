using DeltaEngine.Physics2D.Farseer;

namespace DeltaEngine.Platforms.Tests.ModuleMocks
{
	public class PhysicsMock : BaseMocks
	{
		internal PhysicsMock(AutofacStarterForMockResolver resolver)
			: base(resolver)
		{
			resolver.RegisterSingleton<FarseerPhysics>();
		}
	}
}
