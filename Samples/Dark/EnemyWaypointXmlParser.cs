using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Content.Xml;
using DeltaEngine.Datatypes;

namespace Dark
{
	public class EnemyWaypointXmlParser
	{
		public EnemyWaypointXmlParser(ContentLoader content)
		{
			this.content = content;
		}

		private readonly ContentLoader content;

		public List<List<Vector>> ParseXml()
		{
			var xmlContent = ContentLoader.Load<XmlContent>("EnemyWaypoints");
			var xmlData = xmlContent.Data.GetChild("Waypoints").GetChildren("Waypoint");
			return PopulateXmlData(xmlData);
		}

		private static List<List<Vector>> PopulateXmlData(List<XmlData> xmlData)
		{
			var waypointsList = new List<List<Vector>>();
			List<Vector> waypointSublist = null;
			int currentItem = 0;
			while (currentItem < xmlData.Count)
			{
				var item = xmlData[currentItem];
				var waypointIndex = int.Parse(item.GetAttributeValue("Index"));
				if (waypointIndex == 0)
				{
					if (waypointSublist != null)
						waypointsList.Add(waypointSublist);

					waypointSublist = new List<Vector>();
				}

				waypointSublist.Add(GetWaypointDefinition(item));
				currentItem++;
			}

			waypointsList.Add(waypointSublist);
			return waypointsList;
		}

		private static Vector GetWaypointDefinition(XmlData item)
		{
			var pixelPositionX = int.Parse(item.GetAttributeValue("PositionX"));
			var pixelPositionY = int.Parse(item.GetAttributeValue("PositionY"));
			var worldPoint = Coordinates.PixelToWorld(new Point(pixelPositionX, pixelPositionY));
			return new Vector(worldPoint.X, worldPoint.Y, 0.0f);
		}
	}
}