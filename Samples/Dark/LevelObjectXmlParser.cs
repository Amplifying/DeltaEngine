using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Content.Xml;
using DeltaEngine.Datatypes;

namespace Dark
{
	public class LevelObjectXmlParser
	{
		public LevelObjectXmlParser()
		{
			sceneObjectList = new List<LevelObjectDefinition>();
		}

		private readonly List<LevelObjectDefinition> sceneObjectList;

		public List<LevelObjectDefinition> ParseXml()
		{
			var xmlContent = ContentLoader.Load<XmlContent>("FinalAsylumGroundFloor");
			var xmlData = xmlContent.Data.GetChild("SceneObjects").GetChildren("Object");
			return PopulateXmlData(xmlData);
		}

		private List<LevelObjectDefinition> PopulateXmlData(List<XmlData> xmlData)
		{
			foreach (XmlData item in xmlData)
				sceneObjectList.Add(GetLevelObjectDefinition(item));

			return sceneObjectList;
		}

		private static LevelObjectDefinition GetLevelObjectDefinition(XmlData item)
		{
			return new LevelObjectDefinition
			{
				Position = new Point(float.Parse(item.GetAttributeValue("PositionX")),
					float.Parse(item.GetAttributeValue("PositionY"))),
				Rotation = int.Parse(item.GetAttributeValue("RotationAngle")),
				Width = int.Parse(item.GetAttributeValue("Width")),
				Height = int.Parse(item.GetAttributeValue("Height")),
				Category = item.GetAttributeValue("Category"),
				Filename = item.GetAttributeValue("Filename"),
				Type = item.GetAttributeValue("Type"),
				Room = int.Parse(item.GetAttributeValue("Room"))
			};			
		}
	}
}