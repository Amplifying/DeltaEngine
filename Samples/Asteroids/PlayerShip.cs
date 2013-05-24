using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.Sprites;

namespace Asteroids
{
	/// <summary>
	///   The player's armed spaceship deployed against the dangerous rocks
	/// </summary>
	public class PlayerShip : Sprite
	{
		public PlayerShip(ContentLoader content, AsteroidsGame game)
			: base(
				content.Load<Image>("ship2"), new Rectangle(Point.Half, new Size(.05f, .05f)), Color.White)
		{
			this.game = game;
			Add(new Velocity2D(Point.Zero, Constants.MaximumObjectVelocity));
			Add<PlayerMovementHandler>();
			Add(new TurnAngle(0));
			Add(new Hitpoints(Constants.PlayerMaxHp));
			Add<FullAutoFire>();
			RenderLayer = (int)Constants.RenderLayer.Player;
		}

		internal AsteroidsGame game;

		public bool IsFireing { get; set; }

		public void ShipAccelerate()
		{
			Get<Velocity2D>().Accelerate(Constants.PlayerAcceleration * Time.Current.Delta,
				Rotation);
		}

		public void SteerLeft()
		{
			Rotation -= Constants.PlayerTurnSpeed * Time.Current.Delta;
		}

		public void SteerRight()
		{
			Rotation += Constants.PlayerTurnSpeed * Time.Current.Delta;
		}

		public void Turn(float rotationAmount)
		{
			Rotation = Rotation + rotationAmount < 0
				? 360 + Rotation + rotationAmount
				: (Rotation + rotationAmount > 360
					? Rotation + rotationAmount - 360 : Rotation + rotationAmount);
			Rotation += rotationAmount;
		}
	}
}