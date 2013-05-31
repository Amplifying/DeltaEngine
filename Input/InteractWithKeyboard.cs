using System;
using System.Collections.Generic;
using DeltaEngine.Entities;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Allows entities to respond to keyboard input by sending them a message on a key release
	/// </summary>
	public class InteractWithKeyboard : EntityHandler
	{
		public InteractWithKeyboard(InputCommands input)
		{
			foreach (Key key in Enum.GetValues(typeof(Key)))
				input.Add(key, KeyPressed);
		}

		private void KeyPressed(Key key)
		{
			var entities = EntitySystem.Current.GetEntitiesByHandler(this);
			foreach (var entity in entities)
				entity.MessageAllListeners(new KeyPress(key));
		}

		public class KeyPress
		{
			public KeyPress(Key key)
			{
				Key = key;
			}

			public Key Key { get; private set; }
		}

		public override void Handle(List<Entity> entities) {}
	}
}