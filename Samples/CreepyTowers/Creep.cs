using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;

namespace CreepyTowers
{
	public class Creep : Sprite
	{
		public Creep(ScreenSpace screen, Image creepImage, Rectangle drawArea)
			: base(creepImage, drawArea)
		{
			Add(new MovementData());
			Add<RightMovementHandler>();
			RenderLayer = 1;
		}
	}

	public class MovementData
	{
		public MovementData()
		{
			Speed = 0;
		}

		public float Speed { get; set; }
	}

	public class RightMovementHandler : EntityHandler
	{
		public override void Handle(Entity entity)
		{
			var drawArea = entity.Get<Rectangle>();
			float posX = drawArea.Left;
			if ((posX >= 0.94f))
				return;

			posX += entity.Get<MovementData>().Speed * Time.Current.Delta;
			entity.Set(new Rectangle(posX, drawArea.Top, drawArea.Width, drawArea.Height));
		}

		public override EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.First; }
		}
	}
}