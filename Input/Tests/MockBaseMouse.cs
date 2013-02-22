using DeltaEngine.Datatypes;
using DeltaEngine.Input.Devices;

namespace DeltaEngine.Input.Tests
{
	//ncrunch: no coverage start
	public class MockBaseMouse : BaseMouse
	{
		public override bool IsAvailable
		{
			get { return true; }
		}

		public override void SetPosition(Point newPosition) {}
		public override void Run() {}
		public override void Dispose() {}
	}
}