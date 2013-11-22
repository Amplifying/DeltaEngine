using System.Collections.Generic;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.SampleBrowser
{
	/// <summary>
	/// Data container for SampleItems within the SampleBrowser.
	/// </summary>
	public class Sample
	{
		public Sample(string title, SampleCategory category, string solutionFilePath,
			string projectFilePath, string assemblyFilePath)
		{
			Title = title;
			Category = category;
			if (Category == SampleCategory.Game)
			{
				Description = SampleGameDescriptions.ContainsKey(Title)
					? SampleGameDescriptions[Title] : "Sample Game";
				SetImageUrl(Title);
			}
			else if (Category == SampleCategory.Tutorial)
			{
				Description = TutorialDescritions.ContainsKey(Title)
					? TutorialDescritions[Title] : "Tutorial";
				SetImageUrl("Tutorial");
				Title = Title.SplitWords(false);
			}
			else
			{
				Description = "Visual Test";
				SetImageUrl("VisualTest");
			}
			SolutionFilePath = solutionFilePath;
			ProjectFilePath = projectFilePath;
			AssemblyFilePath = assemblyFilePath;
		}

		private static readonly Dictionary<string, string> SampleGameDescriptions =
			new Dictionary<string, string>
			{
				{
					"Asteroids",
					"A game similar to the old arcade classic. Take control of the alien missile ship and " +
						"split up the infinitely respawning rocks until they vanish, but do not collide to them!"
				},
				{
					"Blocks",
					"A clone of the classic game Tetris, this is all about arranging the falling blocks to " +
						"complete horizontal rows so that they vanish and don't fill up the whole area in height."
				},
				{
					"Breakout",
					"Move the paddle by using your keyboard, mouse or touch control of a mobile device " +
						"and hit the blocks while preventing the ball from crossing the lower border."
				},
				{
					"Drench",
					"Both players try to fill as much of the field as possible by taking over adjacent " +
						"fields of similar colours starting from either the left upper or the right lower corner."
				},
				{
					"GameOfDeath",
					"Stop little rabbits from multiplying and dominating the world. Weapons range from " +
						"simple mallets over fire and poison to atomic bombs. Your goal is to kill them all " +
						"before they overrun you. Based on the Game Of Life game idea."
				},
				{
					"GhostWars",
					"GhostWars is a real-time strategy game (RTS), where you have to conquer all trees " +
						"with ghosts. You can play alone vs AI or over network against your friends."
				},
				{
					"Insight",
					"A simple business app demo as a proof of concept how to use the UI Editor " +
						"for non-games in Milestone 5. "
				},
				{
					"LogoApp",
					"Not as much an actual game as s rather simple application aiming to serve as an " +
						"example on the use of 2D rendering, controls and sound in one short project. Its " +
						"code might be a good starting point to learn about the ways of the Delta Engine " +
						"aside from the tutorials."
				},
				{
					"SideScroller",
					"This game was developed to improve image handling. Basically we wanted to allow " +
						"tile-able textures, which you can see in the background of the game. This feature " +
						"is also used in Basic Tutorial 6: Scrollable Background."
				},
				{
					"Snake",
					"Similar to the game that came to fame from its popularity on mobile devices. " +
						"This one is mostly about 2D polygon rendering and different ways of control " +
						"(try keys, mouse and touch, if available!)"
				}
			};

		private static readonly Dictionary<string, string> TutorialDescritions =
			new Dictionary<string, string>
			{
				{
					"Basic01CreateWindow",
					"The Window is created automatically, but if you want to change it use the " +
						"App.Resolve method, which you can also use to get any engine or game class."
				},
				{
					"Basic02DrawLine",
					"To draw a line create a Line2D object with the start and end position plus a color."
				},
				{
					"Basic03DrawEllipse",
					"The Rendering namespace provides a ton of shape classes you can use or extend. " +
						"Line2D and FilledRect are useful for debugging, but others like Circle or Ellipse " +
						"are also useful."
				},
				{
					"Basic04DrawImage",
					"To load and draw images in 2D use the Sprite class. See the " +
						"Tutorials below for 3D textures."
				},
				{
					"Basic05RotatingSprite",
					"Attach a behavior class to an entity to do something with it at runtime. In this " +
						"example we are just using the Rotate behavior class."
				},
				{
					"Basic06ScrollableBackground",
					"Let's create two sprites, one for a player moving around and the other for the " +
						"scrolling background. This steps shows how to handle the scrolling background, " +
						"see the Entity tutorials for player and enemy classes"
				},
				{
					"Basic07Fonts",
					"Simply speaking FontText is just like all the other entities we met before, it just " +
						"has different constructor parameters. Normally in an application the same font " +
						"content is used many times (e.g. text on lots of buttons), like Sprites can be " +
						"created from Image objects, FontText can be created from Font content objects."
				},
				{
					"Basic08PlaySound",
					"Creating Sounds works like the examples above. We can also play it right away, but " +
						"that is not how sounds are usually used. Instead we want to attach it to some other " +
						"entity and trigger the playback when some condition is true. Or work directly with " +
						"events like in this example."
				},
				{
					"Basic09PlayMusic",
					"Music content is also loaded like any other content. Playback might start right away " +
						"or event based. A better idea is to use the MusicList functionality in the Scene class."
				},
				{
					"Entities01OwnEntity",
					"To create an Entity just derive from it and attach all required data and behaviors " +
						"with the Add, Start and OnDraw methods. You can also use an existing entity class " +
						"and derive from it. For example if you want to create a 2D or 3D Entity to render " +
						"something on screen use Entity2D or Entity3D as your base class."
				},
				{
					"Entities02AttachingBehavior",
					"An entity can have multiple behaviours and these behaviours modify the component data " +
						"of the entities, each frame, if some particular condition is fulfilled. Behaviours " +
						"are added to an entity by the Start<>() method of the Entity Class. "
				},
				{
					"Entities03AddingComponents",
					"An entity can have multiple components and these components usually consist of the " +
						"data that is associated with the entity.  Components can be added by calling the " +
						"Add() method of the Entity Class."
				},
				{
					"Entities04ChangingBehavior",
					"An entity can have multiple behaviours and each behaviour has the power to change " +
						"entity component values."
				},
				{
					"Entities05Tags",
					"Every entity can have a custom tag attached to it. This allows to quickly query " +
						"through the list of entities as well and can be used for other specific user needs. " +
						"An entity can have multiple tags. You can freely add and remove tags with AddTag " +
						"and RemoveTag."
				},
				{
					"Entities06Triggers",
					"An Trigger is a behaviour which does a fixed predetermined task. Delta Engine has 2 " +
						"pre-coded triggers which can be directly added as behaviours to an entity, namely: " +
						"CollisionTrigger and TimeTrigger."
				},
				{
					"Entities07DeactivatingEntities",
					"All the entities in the game can be activated and deactivated at will. If an entity " +
						"is deactivated then it is not handled by the behaviors anymore and hence is " +
						"non-existent (neither rendered nor updated). When an entity is created, it is " +
						"active by default. To pause just updating you can also pause an entity or " +
						"temporarily remove an update behavior."
				},
				{
					"Entities08InputCommands",
					"The user can request the system to give a list of all entities that have been " +
						"assigned a particular tag. This allows the user to filter through the entire " +
						"entity list and get specific results."
				},
				{
					"Entities09Enemies",
					"The user can request the system to give a list of all entities of a specific custom " +
						"type. This allows the user to filter through the entire entity list and get only " +
						"entities of a specific type."
				},
				{
					"Entities10LogoRacer",
					"The final entities tutorial brings it all together in a little sample game called " +
						"LogoRacer. You have to avoid the spinning earths as long as possible to get more points."
				}
			};

		public string Title { get; private set; }
		public string Description { get; private set; }
		public SampleCategory Category { get; private set; }
		public string ImageUrl { get; private set; }
		public string SolutionFilePath { get; private set; }
		public string ProjectFilePath { get; private set; }
		public string AssemblyFilePath { get; private set; }
		public string EntryClass { get; set; }
		public string EntryMethod { get; set; }

		private void SetImageUrl(string fileName)
		{
			ImageUrl = GetIconWebPath() + fileName + ".png";
		}

		private static string GetIconWebPath()
		{
			return "http://DeltaEngine.net/Editor/Icons/";
		}

		public bool ContainsFilterText(string filterText)
		{
			return Title.ContainsCaseInsensitive(filterText) ||
				Category.ToString().ContainsCaseInsensitive(filterText) ||
				Description.ContainsCaseInsensitive(filterText) ||
				AssemblyFilePath.ContainsCaseInsensitive(filterText) ||
				EntryMethod.ContainsCaseInsensitive(filterText);
		}

		public override string ToString()
		{
			return "Sample: " + "Title=" + Title + ", Category=" + Category + ", Description=" +
				Description;
		}
	}
}