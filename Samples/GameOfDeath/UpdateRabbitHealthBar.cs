using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace GameOfDeath
{
	/// <summary>
	/// Makes a rabbit's health bar follow the rabbit sprite, plus updates its size and color
	/// according to the rabbit's health
	/// </summary>
	public class UpdateRabbitHealthBar : EntityListener
	{
		public override void ReceiveMessage(Entity entity, object message)
		{
			if (message is MoveRabbit.HasMoved)
				UpdateHealthBar((Rabbit)entity);
		}

		private void UpdateHealthBar(Rabbit rabbit)
		{
			UpdateDrawArea(rabbit);
			UpdateColor(rabbit);
		}

		private static void UpdateDrawArea(Rabbit rabbit)
		{
			Rectangle drawArea = rabbit.RabbitSprite.DrawArea;
			rabbit.RabbitHealthBar.DrawArea = Rectangle.FromCenter(drawArea.Center.X,
				drawArea.Top - 0.01f, rabbit.HealthPercentage * drawArea.Width * 0.5f, HealthBarHeight);
		}

		private const float HealthBarHeight = 10.0f / 1920;

		private static void UpdateColor(Rabbit rabbit)
		{
			var percentage = rabbit.HealthPercentage;
			rabbit.RabbitHealthBar.Color = percentage < 0.25f ? Red : percentage < 0.5f ? Yellow : Green;
		}

		private static readonly Color Red = new Color(96, 0, 0, 128);
		private static readonly Color Yellow = new Color(96, 96, 0, 128);
		private static readonly Color Green = new Color(0, 96, 0, 128);

		public override EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.First; }
		}
	}
}