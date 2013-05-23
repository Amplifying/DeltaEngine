using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Rendering;

namespace Blobs.Creatures
{
	/// <summary>
	/// Consists of a bouncy body with two animated eyes and eyebrows
	/// </summary>
	public class Blob : RubberBall, IDisposable
	{
		public Blob(EntitySystem entitySystem, ScreenSpace screen, InputCommands input)
		{
			this.entitySystem = entitySystem;
			this.screen = screen;
			this.input = input;
			camera = screen as Camera2DControlledQuadraticScreenSpace;
			RenderLayer = 10;
			CreateFacialElements();
			RespondToMouseMovement();
		}

		protected readonly EntitySystem entitySystem;
		private readonly ScreenSpace screen;
		protected readonly InputCommands input;
		protected readonly Camera2DControlledQuadraticScreenSpace camera;

		private void CreateFacialElements()
		{
			Mood = new Mood();
			LeftEye = new Eye(entitySystem, screen, input, Mood);
			RightEye = new Eye(entitySystem, screen, input, Mood);
			entitySystem.Add(LeftEyebrow = new LeftEyebrow(LeftEye, Mood));
			entitySystem.Add(RightEyebrow = new RightEyebrow(RightEye, Mood));
		}

		private Mood Mood { get; set; }
		private LeftEyebrow LeftEyebrow { get; set; }
		private RightEyebrow RightEyebrow { get; set; }
		private Eye LeftEye { get; set; }
		private Eye RightEye { get; set; }

		private void RespondToMouseMovement()
		{
			mouseMovementCommand = input.AddMouseMovement(StoreMousePosition);
		}

		private Command mouseMovementCommand;

		private void StoreMousePosition(Mouse mouse)
		{
			Mood.Fear = mouse.Position.DistanceTo(camera.Transform(Center));
		}

		public override void Run()
		{
			base.Run();
			ProcessColorChange(Time.Current.Delta);
			ProcessBlink(Time.Current.Delta);
			PositionFacialElements();
			if (HasDied)
				Dispose();
		}

		private void ProcessColorChange(float delta)
		{
			if (!isChangingColor)
				return;

			colorChangeElapsed += delta;
			Color = Color.Lerp(colorFrom, colorTo, colorChangeElapsed / ColorChangeDuration);
			if (colorChangeElapsed < ColorChangeDuration)
				return;

			Color = colorTo;
			isChangingColor = false;
		}

		private bool isChangingColor;
		private float colorChangeElapsed = ColorChangeDuration;
		private const float ColorChangeDuration = 1.0f;
		private Color colorFrom;
		private Color colorTo;

		private void ProcessBlink(float delta)
		{
			if (IsNeedingToBlink(delta))
				BeginBlink();

			if (IsNeedingToEndBlink(delta))
				EndBlink();
		}

		private static bool IsNeedingToBlink(float delta)
		{
			return (Randomizer.Current.Get() < delta);
		}

		private void BeginBlink()
		{
			LeftEye.IsBlinking = true;
			RightEye.IsBlinking = true;
			blinkElapsed = 0.0f;
			blinkTime = 0.1f;
		}

		private float blinkElapsed;
		private float blinkTime;

		private bool IsNeedingToEndBlink(float delta)
		{
			blinkElapsed += delta;
			return blinkElapsed >= blinkTime;
		}

		private void EndBlink()
		{
			LeftEye.IsBlinking = false;
			RightEye.IsBlinking = false;
		}

		private void PositionFacialElements()
		{
			PositionLeftEye();
			PositionRightEye();
		}

		private void PositionLeftEye()
		{
			var eyeCenter = new Point(Center.X - RadiusX / 2, Center.Y);
			eyeCenter.RotateAround(Center, Rotation);
			LeftEye.Center = eyeCenter;
			LeftEye.Rotation = Rotation;
			LeftEye.RadiusX = (RadiusX + RadiusY) / 6;
			LeftEye.RadiusY = (RadiusX + RadiusY) / 8;
		}

		private void PositionRightEye()
		{
			var eyeCenter = new Point(Center.X + RadiusX / 2, Center.Y);
			eyeCenter.RotateAround(Center, Rotation);
			RightEye.Center = eyeCenter;
			RightEye.Rotation = Rotation;
			RightEye.RadiusX = (RadiusX + RadiusY) / 6;
			RightEye.RadiusY = (RadiusX + RadiusY) / 8;
		}

		public void UpdateColor(float playerRadius)
		{
			colorFrom = Color;
			colorTo = playerRadius >= Radius ? weakColor : strongColor;
			colorChangeElapsed = Color == Color.White ? ColorChangeDuration : 0.0f;
			if (colorFrom != colorTo)
				isChangingColor = true;
		}

		private readonly Color weakColor = new Color(40, 150, 40);
		private readonly Color strongColor = new Color(255, 0, 0);

		public virtual void Dispose()
		{
			input.Remove(mouseMovementCommand);
			LeftEyebrow.IsActive = false;
			RightEyebrow.IsActive = false;
			LeftEye.Dispose();
			RightEye.Dispose();
			IsActive = false;
		}
	}
}