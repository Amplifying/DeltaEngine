using System;
using System.IO;
using DeltaEngine.Commands;
using DeltaEngine.Core;

namespace DeltaEngine.Content.Xml
{
	/// <summary>
	/// Commands from a content file which trigger input events.
	/// </summary>
	public class InputCommands : XmlContent
	{
		public InputCommands(string contentName)
			: base(contentName) {}

		protected override void LoadData(Stream fileData)
		{
			base.LoadData(fileData);
			foreach (var child in Data.Children)
				Command.Register(child.GetAttributeValue("Name"), ParseTriggers(child));
		}

		private static Trigger[] ParseTriggers(XmlData command)
		{
			var triggers = new Trigger[command.Children.Count];
			for (int index = 0; index < command.Children.Count; index++)
			{
				var triggerNode = command.Children[index];
				var triggerType = triggerNode.Name.GetTypeFromShortNameOrFullNameIfNotFound();
				if (triggerType == null)
					throw new UnableToCreateTriggerTypeIsUnknown(triggerNode.Name);
				triggers[index] = Activator.CreateInstance(triggerType, triggerNode.Value) as Trigger;
			}
			return triggers;
		}

		private class UnableToCreateTriggerTypeIsUnknown : Exception
		{
			public UnableToCreateTriggerTypeIsUnknown(string name)
				: base(name) {}
		}
	}
}