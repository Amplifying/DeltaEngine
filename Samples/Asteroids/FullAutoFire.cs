using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Entities;

namespace Asteroids
{
	internal class FullAutoFire : EntityHandler
	{
		public FullAutoFire()
		{
			CadenceShotsPerSec = Constants.PlayerCadance;
			timeLastShot = Time.Current.Milliseconds;
		}

		public float CadenceShotsPerSec { get; private set; }
		private float timeLastShot;

		public override void Handle(List<Entity> entities)
		{
			foreach (PlayerShip entity in entities.OfType<PlayerShip>())
				if (entity.IsFireing && Time.Current.Milliseconds - 1 / CadenceShotsPerSec > timeLastShot)
				{
					var projectile = new Projectile(entity.DrawArea.Center, entity.Rotation);
					timeLastShot = Time.Current.Milliseconds;
					entity.game.existantProjectiles.Add(projectile);
				}
		}

		public override EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.First; }
		}
	}
}