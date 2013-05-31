using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering.ScreenSpaces;

namespace VehiclePhysicsGame
{
	public class VehiclePhysicsGame
	{
		public VehiclePhysicsGame(ContentLoader contentLoader, ScreenSpace screenSpace,
			Physics physics, InputCommands inputCommands)
		{
			this.screenSpace = screenSpace;
			this.contentLoader = contentLoader;
			this.physics = physics;
			this.inputCommands = inputCommands;
			Initialize();
			AddControls();
		}

		private void AddControls()
		{
			inputCommands.Add(Key.CursorUp, State.Pressed, key => Vehicle.Accelerate());
			inputCommands.Add(Key.CursorUp, State.Pressing, key => Vehicle.Accelerate());

			inputCommands.Add(Key.CursorDown, State.Pressing, key => Vehicle.Brake());
			inputCommands.Add(Key.CursorDown, State.Pressing, key => Vehicle.Brake());
		}

		private void Initialize()
		{
			Terrain = new Terrain(physics);
			Terrain.CreateDefaultTerrain();
			Vehicle = new Vehicle(screenSpace.Viewport.Center,physics,contentLoader);
		}

		private ScreenSpace screenSpace;
		private readonly ContentLoader contentLoader;
		private readonly Physics physics;
		private readonly InputCommands inputCommands;

		public Terrain Terrain { get; private set; }
		public Vehicle Vehicle { get; private set; }
	}
}
