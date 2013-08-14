namespace DeltaEngine.Rendering.Tests
{
	internal static class Program
	{
		public static void Main()
		{
			var tests = new ModelTests();
			tests.InitializeResolver();
			//tests.RayPick();
			tests.RunTestAndDisposeResolverWhenDone();
		}
	}
}
