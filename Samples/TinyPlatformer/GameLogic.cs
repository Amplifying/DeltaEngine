using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using System.Collections.Generic;
using DeltaEngine.Rendering;
using DeltaEngine.Input;

namespace TinyPlatformer
{
	public class GameLogic : Entity2D
	{
		/// <summary>
		/// Start a handler and traverse the objects, for update end render.
		/// </summary>
		public GameLogic(Map map, InputCommands inputCommands)
			: base(Rectangle.Zero)
		{
			this.actorList = map.ActorList;
			player = map.Player;
			input = new GameInput(inputCommands);
			BindPlayerToControls();
			Start<GameUpdate>();
		}

		public Actor player;

		private void BindPlayerToControls()
		{
			input.LeftDown += () => { player.Left = true; };
			input.RightDown += () => { player.Right = true; };
			input.JumpDown += () => { player.Jump = true; };
			input.LeftUp += () => { player.Left = false; };
			input.RightUp += () => { player.Right = false; };
			input.JumpUp += () => { player.Jump = false; };
		}

		internal class GameUpdate : Behavior2D
		{
			public override void Handle(Entity2D entity)
			{
				deltaTime += Math.Min(1.0f, Time.Current.Delta);
				var gameLogic = entity as GameLogic;
				while (deltaTime > Constants.TimeStep)
				{
					deltaTime = deltaTime - Constants.TimeStep;
					foreach (var actor in gameLogic.actorList)
					{
						actor.Update(Constants.TimeStep);
					}
				}
				foreach (var actor in gameLogic.actorList)
				{
					actor.Render(deltaTime);
				}

			}
			private float deltaTime;
		}
		private List<Actor> actorList;
		private GameInput input;
	}
}