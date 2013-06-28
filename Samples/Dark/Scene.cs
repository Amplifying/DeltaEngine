using DeltaEngine.Content;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Rendering.ScreenSpaces;

namespace Dark
{
	public abstract class Scene
	{
		protected Scene(Game game, InputCommands input, ScreenSpace screenSpace)
		{
			this.game = game;
			this.content = content;
			this.input = input;
			this.entitySystem = entitySystem;
			this.screenSpace = screenSpace;
		}

		protected readonly Game game;
		protected readonly ContentLoader content;
		protected readonly InputCommands input;
		protected readonly EntitySystem entitySystem;
		protected readonly ScreenSpace screenSpace;

		public abstract void Load();
		public abstract void Release();

		public abstract void Update();

		protected int updateCount;
	}
}