using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Xml.Linq;
using DeltaEngine.Commands;
using DeltaEngine.Content.Xml;
using DeltaEngine.Editor.Core;
using DeltaEngine.Extensions;
using DeltaEngine.Input;

namespace DeltaEngine.Editor.InputEditor
{
	internal class InputSaverAndLoader
	{
		public void SaveInput(CommandList commandList, Service service)
		{
			var root = CreateMainRoot("InputCommands");
			foreach (var command in commandList.GetCommands())
				SetCommand(root, command, commandList.GetAllTriggers(command));
			var file = new XmlFile(root);
			string xmlDataString = file.Root.ToXmlString();
			File.WriteAllText("Content/" + service.ProjectName + "/InputCommands.xml",
				xmlDataString);
		}

		public XmlData CreateMainRoot(string filename)
		{
			var root = new XmlData(filename);
			root.Name = "InputMetaData";
			root.AddAttribute("Name", "DeltaEngine.Editor.InputCommandEditor");
			root.AddAttribute("Type", "Scene");
			root.AddAttribute("LastTimeUpdated", DateTime.Now.GetIsoDateTime());
			root.AddAttribute("ContentDeviceName", "Delta");
			return root;
		}

		public void SetCommand(XmlData root, string command, List<Trigger> triggers)
		{
			var child = new XmlData("Command");
			child.AddAttribute("Name", command);
			foreach (Trigger trigger in triggers)
				SetTrigger(trigger, child);
			root.AddChild(child);
		}

		private static void SetTrigger(Trigger trigger, XmlData xmlData)
		{
			var child = new XmlData("Trigger");
			if (trigger.GetType() == typeof(KeyTrigger))
				CreateKeyButtonAttributes(trigger, child);
			if (trigger.GetType() == typeof(MouseButtonTrigger))
				CreateMouseButtonAttributes(trigger, child);
			if (trigger.GetType() == typeof(GamePadButtonTrigger))
				CreateGamepadButtonAttributes(trigger, child);
			if (trigger.GetType() == typeof(TouchPressTrigger))
				child.AddAttribute("State", (trigger as TouchPressTrigger).State);
			xmlData.AddChild(child);
		}

		private static void CreateKeyButtonAttributes(Trigger trigger, XmlData xmlData)
		{
			xmlData.AddAttribute("Type", "KeyTrigger");
			xmlData.AddAttribute("Button", (trigger as KeyTrigger).Key);
			xmlData.AddAttribute("State", (trigger as KeyTrigger).State);
		}

		private static void CreateMouseButtonAttributes(Trigger trigger, XmlData xmlData)
		{
			xmlData.AddAttribute("Type", "MouseTrigger");
			xmlData.AddAttribute("Button", (trigger as MouseButtonTrigger).Button);
			xmlData.AddAttribute("State", (trigger as MouseButtonTrigger).State);
		}

		private static void CreateGamepadButtonAttributes(Trigger trigger, XmlData xmlData)
		{
			xmlData.AddAttribute("Type", "GamepadTrigger");
			xmlData.AddAttribute("Button", (trigger as GamePadButtonTrigger).Button);
			xmlData.AddAttribute("State", (trigger as GamePadButtonTrigger).State);
		}

		public void LoadInput(InputEditorViewModel editor)
		{
			var metadataXml =
				XDocument.Load(Path.Combine("Content/" + editor.service.ProjectName, "InputCommands.xml"));
			foreach (XElement contentElement in metadataXml.Root.Elements())
			{
				var commandName = contentElement.FirstAttribute;
				editor.NewCommand = commandName.Value;
				editor.AddNewCommand();
				foreach (var element in contentElement.Elements())
				{
					if (element.FirstAttribute.Value == "KeyTrigger")
						LoadKeyCommand(element, editor, commandName.Value);
					if (element.FirstAttribute.Value == "MouseTrigger")
						LoadMouseCommand(element, editor, commandName.Value);
					if (element.FirstAttribute.Value == "GamepadTrigger")
						LoadGamepadCommand(element, editor, commandName.Value);
				}
			}
		}

		private static void LoadKeyCommand(XElement element, InputEditorViewModel editor,
			string commandName)
		{
			var attributes = element.Attributes();
			var attributeList = new List<XAttribute>();
			foreach (var attribute in attributes)
				attributeList.Add(attribute);
			editor.availableCommands.AddTrigger(commandName,
				((Key)Enum.Parse(typeof(Key), attributeList[1].Value)),
				((State)Enum.Parse(typeof(State), attributeList[2].Value)));
		}

		private static void LoadMouseCommand(XElement element, InputEditorViewModel editor,
			string commandName)
		{
			var attributes = element.Attributes();
			var attributeList = new List<XAttribute>();
			foreach (var attribute in attributes)
				attributeList.Add(attribute);
			editor.availableCommands.AddTrigger(commandName,
				((MouseButton)Enum.Parse(typeof(MouseButton), attributeList[1].Value)),
				((State)Enum.Parse(typeof(State), attributeList[2].Value)));
		}

		private static void LoadGamepadCommand(XElement element, InputEditorViewModel editor,
			string commandName)
		{
			var attributes = element.Attributes();
			var attributeList = new List<XAttribute>();
			foreach (var attribute in attributes)
				attributeList.Add(attribute);
			editor.availableCommands.AddTrigger(commandName,
				((GamePadButton)Enum.Parse(typeof(GamePadButton), attributeList[1].Value)),
				((State)Enum.Parse(typeof(State), attributeList[2].Value)));
		}
	}
}