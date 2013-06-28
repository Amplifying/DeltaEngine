using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Rendering.Sprites;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests
{
	public class SceneTests : TestWithMocksOrVisually
	{
		[Test]
		public void AddingControlAddsToListOfControls()
		{
			var scene = new Scene();
			Assert.AreEqual(0, scene.Controls.Count);
			var control = new EmptyControl();
			scene.Add(control);
			Assert.AreEqual(1, scene.Controls.Count);
			Assert.AreEqual(control, scene.Controls[0]);
		}

		public class EmptyControl : Entity {}

		[Test]
		public void AddingControlTwiceOnlyAddsItOnce()
		{
			var scene = new Scene();
			var control = new EmptyControl();
			scene.Add(control);
			scene.Add(control);
			Assert.AreEqual(1, scene.Controls.Count);
		}

		[Test]
		public void AddingControlToActiveSceneActivatesIt()
		{
			var image = ContentLoader.Load<Image>("DeltaEngineLogo");
			var label = new Sprite(image, Rectangle.One);
			var scene = new Scene();
			scene.Show();
			scene.Show();
			scene.Add(label);
			Assert.IsTrue(label.IsActive);
			Window.CloseAfterFrame();
		}

		[Test]
		public void AddingControlToInactiveSceneDeactivatesIt()
		{
			var image = ContentLoader.Load<Image>("DeltaEngineLogo");
			var label = new Sprite(image, Rectangle.One) { IsActive = true };
			var scene = new Scene();
			scene.Hide();
			scene.Add(label);
			Assert.IsFalse(label.IsActive);
			Window.CloseAfterFrame();
		}

		[Test]
		public void RemovingControlRemovesFromListOfControls()
		{
			var image = ContentLoader.Load<Image>("DeltaEngineLogo");
			var label = new Sprite(image, Rectangle.One);
			var scene = new Scene();
			scene.Add(label);
			scene.Remove(label);
			Assert.AreEqual(0, scene.Controls.Count);
			Window.CloseAfterFrame();
		}

		[Test]
		public void RemovingControlDeactivatesIt()
		{
			var scene = new Scene();
			var image = ContentLoader.Load<Image>("DeltaEngineLogo");
			var label = new Sprite(image, Rectangle.One) { IsActive = true };
			scene.Add(label);
			scene.Remove(label);
			Assert.IsFalse(label.IsActive);
			Window.CloseAfterFrame();
		}

		[Test]
		public void ClearingControlsDeactivatesThem()
		{
			var image = ContentLoader.Load<Image>("DeltaEngineLogo");
			var label = new Sprite(image, Rectangle.One) { IsActive = true };
			var control = new EmptyControl { IsActive = true };
			var scene = ActivateSceneAndAddControls(new List<Entity> { label, control });
			scene.Clear();
			Assert.AreEqual(0, scene.Controls.Count);
			Assert.IsFalse(label.IsActive);
			Assert.IsFalse(control.IsActive);
			Window.CloseAfterFrame();
		}

		private static Scene ActivateSceneAndAddControls(IEnumerable<Entity> controls)
		{
			var scene = new Scene();
			foreach (Entity control in controls)
				scene.Add(control);

			return scene;
		}

		[Test]
		public void HidingSceneDeactivatesControls()
		{
			var image = ContentLoader.Load<Image>("DeltaEngineLogo");
			var label = new Sprite(image, Rectangle.One) { IsActive = true };
			var control = new EmptyControl { IsActive = true };
			var scene = ActivateSceneAndAddControls(new List<Entity> { label, control });
			scene.Hide();
			scene.Hide();
			Assert.AreEqual(2, scene.Controls.Count);
			Assert.IsFalse(label.IsActive);
			Assert.IsFalse(control.IsActive);
			Window.CloseAfterFrame();
		}

		[Test]
		public void ControlsDontRespondToInputWhenSceneIsHidden()
		{
			var button = CreateHiddenSceneWithButton();
			bool pressed = false;
			button.Messaged += message =>
			{
				if (message is Interact.ControlPressed)
					pressed = true;
			};
			SetMouseState(State.Pressing, Point.Half);
			Assert.IsFalse(pressed);
			Assert.AreEqual(NormalColor, button.Color);
			Window.CloseAfterFrame();
		}

		private static readonly Color NormalColor = Color.LightGray;

		private static Button CreateHiddenSceneWithButton()
		{
			var button = CreateButton();
			var scene = new Scene();
			scene.Add(button);
			scene.Hide();
			return button;
		}

		private static Button CreateButton()
		{
			var logo = ContentLoader.Load<Image>("DeltaEngineLogo");
			var theme = new Theme
			{
				Button = new Theme.Appearance(logo, NormalColor),
				ButtonMouseover = new Theme.Appearance(logo, MouseoverColor),
				ButtonPressed = new Theme.Appearance(logo, PressedColor),
				Font = new Font("Verdana12")
			};
			return new Button(theme, Small);
		}

		private static readonly Color MouseoverColor = Color.White;
		private static readonly Color PressedColor = Color.Red;

		private void SetMouseState(State state, Point position)
		{
			Resolve<MockMouse>().SetMousePositionNextFrame(position);
			Resolve<MockMouse>().SetMouseButtonStateNextFrame(MouseButton.Left, state);
			resolver.AdvanceTimeAndExecuteRunners();
		}

		[Test]
		public void DrawButtonWhichChangesColorAndSizeAndSpinsOnHover()
		{
			var scene = new Scene();
			var button = CreateButton();
			button.Start<SpinIfHovering, ChangeSizeDynamically>();
			scene.Add(button);
		}

		private static readonly Rectangle Small = Rectangle.FromCenter(0.5f, 0.5f, 0.3f, 0.1f);
		private static readonly Rectangle Big = Rectangle.FromCenter(0.5f, 0.5f, 0.36f, 0.12f);

		private class SpinIfHovering : Behavior2D
		{
			public override void Handle(Entity2D entity)
			{
				if (entity.Get<Interact.State>().IsHovering)
					entity.Rotation += Time.Current.Delta * SpinRate;
			}

			private const int SpinRate = 180;
		}

		private class ChangeSizeDynamically : EventListener2D
		{
			public override void ReceiveMessage(Entity2D entity, object message)
			{
				if (!entity.Contains<Interact.State>())
					return;

				var state = entity.Get<Interact.State>();
				if (state.IsInside && !state.IsPressed)
					entity.Set(Big);
				else
					entity.Set(Small);
			}

			public override Priority Priority
			{
				get { return Priority.Low; }
			}
		}

		[Test]
		public void ChangeBackgroundImage()
		{
			var scene = new Scene();
			Assert.AreEqual(0, scene.Controls.Count);
			var background = ContentLoader.Load<Image>("SimpleMainMenuBackground");
			scene.SetBackground(background);
			Assert.AreEqual(1, scene.Controls.Count);
			Assert.AreEqual(background, ((Sprite)scene.Controls[0]).Image);
			var logo = ContentLoader.Load<Image>("DeltaEngineLogo");
			scene.SetBackground(logo);
			Assert.AreEqual(1, scene.Controls.Count);
			Assert.AreEqual(logo, ((Sprite)scene.Controls[0]).Image);
			Window.CloseAfterFrame();
		}

		[Test]
		public void BackgroundImageChangesWhenButtonClicked()
		{
			var scene = new Scene();
			scene.SetBackground(ContentLoader.Load<Image>("SimpleSubMenuBackground"));
			var button = CreateButton();
			button.Clicked +=
				() => scene.SetBackground(ContentLoader.Load<Image>("SimpleMainMenuBackground"));
			scene.Add(button);
		}
	}
}