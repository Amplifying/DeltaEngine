using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Platforms.Mocks
{
	internal class MockDevice : Device
	{
		public void Run() {}
		public void Present() {}
		public void Dispose() {}
		public void SetProjectionMatrix(Matrix matrix) {}
		public void SetModelViewMatrix(Matrix matrix) {}
	}
}