using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Entities.Tests;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Rendering.Sprites;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests
{
	public class SceneTests : TestWithAllFrameworks
	{
		[Test]
		public void AddingControlAddsToListOfControls()
		{
			var scene = new Scene();
			Assert.AreEqual(0, scene.Controls.Count);
			var control = new EmptyEntity();
			scene.Add(control);
			Assert.AreEqual(1, scene.Controls.Count);
			Assert.AreEqual(control, scene.Controls[0]);
		}

		[Test]
		public void AddingControlTwiceOnlyAddsItOnce()
		{
			var scene = new Scene();
			var control = new EmptyEntity();
			scene.Add(control);
			scene.Add(control);
			Assert.AreEqual(1, scene.Controls.Count);
		}

		[IntegrationTest]
		public void AddingControlToActiveSceneActivatesIt(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var image = content.Load<Image>("DeltaEngineLogo");
				var label = new Sprite(image, Rectangle.One);
				var scene = new Scene();
				scene.Show();
				scene.Show();
				scene.Add(label);
				Assert.IsTrue(label.IsActive);
			});
		}

		[IntegrationTest]
		public void AddingControlToInactiveSceneDeactivatesIt(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var image = content.Load<Image>("DeltaEngineLogo");
				var label = new Sprite(image, Rectangle.One) { IsActive = true };
				var scene = new Scene();
				scene.Add(label);
				Assert.IsFalse(label.IsActive);
			});
		}

		[IntegrationTest]
		public void RemovingControlRemovesFromListOfControls(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var image = content.Load<Image>("DeltaEngineLogo");
				var label = new Sprite(image, Rectangle.One);
				var scene = new Scene();
				scene.Add(label);
				scene.Remove(label);
				Assert.AreEqual(0, scene.Controls.Count);
			});
		}

		[IntegrationTest]
		public void RemovingControlDeactivatesIt(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var scene = new Scene();
				scene.Show();
				var image = content.Load<Image>("DeltaEngineLogo");
				var label = new Sprite(image, Rectangle.One) { IsActive = true };
				scene.Add(label);
				scene.Remove(label);
				Assert.IsFalse(label.IsActive);
			});
		}

		[IntegrationTest]
		public void ClearingControlsDeactivatesThem(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var image = content.Load<Image>("DeltaEngineLogo");
				var label = new Sprite(image, Rectangle.One) { IsActive = true };
				var control = new EmptyEntity { IsActive = true };
				var scene = ActivateSceneAndAddControls(new List<Entity> { label, control });
				scene.Clear();
				Assert.AreEqual(0, scene.Controls.Count);
				Assert.IsFalse(label.IsActive);
				Assert.IsFalse(control.IsActive);
			});
		}

		private static Scene ActivateSceneAndAddControls(IEnumerable<Entity> controls)
		{
			var scene = new Scene();
			scene.Show();
			foreach (Entity control in controls)
				scene.Add(control);

			return scene;
		}

		[IntegrationTest]
		public void HidingSceneDeactivatesControls(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var image = content.Load<Image>("DeltaEngineLogo");
				var label = new Sprite(image, Rectangle.One) { IsActive = true };
				var control = new EmptyEntity { IsActive = true };
				var scene = ActivateSceneAndAddControls(new List<Entity> { label, control });
				scene.Hide();
				scene.Hide();
				Assert.AreEqual(2, scene.Controls.Count);
				Assert.IsFalse(label.IsActive);
				Assert.IsFalse(control.IsActive);
			});
		}

		[IntegrationTest]
		public void ControlsDontRespondToInputWhenSceneIsHidden(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var button = CreateHiddenSceneWithButton(content);
				bool pressed = false;
				button.Messaged += message =>
				{
					if (message is Interact.ControlPressed)
						pressed = true;
				};
				SetMouseState(State.Pressing, Point.Half);
				Assert.IsFalse(pressed);
				Assert.AreEqual(NormalColor, button.Color);
			});
		}

		private static readonly Color NormalColor = Color.LightGray;

		private static Button CreateHiddenSceneWithButton(ContentLoader content)
		{
			var button = CreateButton(content);
			var scene = new Scene();
			scene.Add(button);
			scene.Show();
			scene.Hide();
			return button;
		}

		private static Button CreateButton(ContentLoader content)
		{
			var logo = content.Load<Image>("DeltaEngineLogo");
			var theme = new Theme
			{
				Button = new Theme.Appearance(logo, NormalColor),
				ButtonMouseover = new Theme.Appearance(logo, MouseoverColor),
				ButtonPressed = new Theme.Appearance(logo, PressedColor),
				Font = new Font(content, "Verdana12")
			};
			return new Button(theme, Small);
		}

		private static readonly Color MouseoverColor = Color.White;
		private static readonly Color PressedColor = Color.Red;

		private void SetMouseState(State state, Point position)
		{
			mockResolver.input.SetMousePosition(position);
			mockResolver.input.SetMouseButtonState(MouseButton.Left, state);
			mockResolver.AdvanceTimeAndExecuteRunners();
		}

		[VisualTest]
		public void DrawButtonWhichChangesColorAndSizeAndSpinsOnHover(Type resolver)
		{
			Start(resolver, (Scene scene, ContentLoader content) =>
			{
				var button = CreateButton(content);
				button.Add<SpinIfHovering, ChangeSizeDynamically>();
				scene.Add(button);
				scene.Show();
			});
		}

		private static readonly Rectangle Small = Rectangle.FromCenter(0.5f, 0.5f, 0.3f, 0.1f);
		private static readonly Rectangle Big = Rectangle.FromCenter(0.5f, 0.5f, 0.36f, 0.12f);

		private class SpinIfHovering : EntityHandler
		{
			public override void Handle(Entity entity)
			{
				if (entity.Get<Interact.State>().IsHovering)
					entity.Set(entity.Get<float>() + Time.Current.Delta * SpinRate);
			}

			private const int SpinRate = 180;
		}

		private class ChangeSizeDynamically : EntityListener
		{
			public override void ReceiveMessage(Entity entity, object message)
			{
				if (!entity.Contains<Interact.State>())
					return;

				var state = entity.Get<Interact.State>();
				if (state.IsInside && !state.IsPressed)
					entity.Set(Big);
				else
					entity.Set(Small);
			}

			public override EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.Low; }
			}
		}

		[IntegrationTest]
		public void ChangeBackgroundImage(Type resolver)
		{
			Start(resolver, (Scene scene, ContentLoader content) =>
			{
				Assert.AreEqual(0, scene.Controls.Count);
				var background = content.Load<Image>("SimpleMainMenuBackground");
				scene.SetBackground(background);
				Assert.AreEqual(1, scene.Controls.Count);
				Assert.AreEqual(background, ((Sprite)scene.Controls[0]).Image);
				var logo = content.Load<Image>("DeltaEngineLogo");
				scene.SetBackground(logo);
				Assert.AreEqual(1, scene.Controls.Count);
				Assert.AreEqual(logo, ((Sprite)scene.Controls[0]).Image);
			});
		}

		[VisualTest]
		public void BackgroundImageChangesWhenButtonClicked(Type resolver)
		{
			Start(resolver, (Scene scene, ContentLoader content) =>
			{
				scene.SetBackground(content.Load<Image>("SimpleSubMenuBackground"));
				var button = CreateButton(content);
				button.Clicked +=
					() => scene.SetBackground(content.Load<Image>("SimpleMainMenuBackground"));
				scene.Add(button);
				scene.Show();
			});
		}
	}
}