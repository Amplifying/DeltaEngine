using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;

namespace CreepyTowers
{
	public class Tower : Sprite
	{
		public Tower(ScreenSpace screen, Image towerImage, Rectangle towerDrawArea)
			: base(towerImage, towerDrawArea)
		{
			Add(new Properties());
			Add<TowerHandler>();
			RenderLayer = 1;

		}

		public class Properties
		{
			
		}


		internal class TowerHandler : EntityHandler
		{
			public override void Handle(Entity entity) {}
		}

		public void FireWhenEnemyInRange()
		{
			
		}
	}
}