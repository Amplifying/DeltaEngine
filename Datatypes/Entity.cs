using System;
using System.Collections.Generic;
using System.IO;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Basis for entities, which have a name and can have components and be based on a template.
	/// https://deltaengine.fogbugz.com/default.asp?W195
	/// </summary>
	public class Entity : IEquatable<Entity>, BinaryData
	{
		public Entity(string name, Entity basedOn = null)
		{
			Name = name;
			BasedOn = basedOn;
		}

		public string Name { get; private set; }
		public Entity BasedOn { get; private set; }

		protected Entity()
		{
			Name = "";
		}

		public void LoadData(BinaryReader reader)
		{
			Name = reader.ReadString();
			int numberOfComponents = reader.ReadInt32();
			components.Clear();
			for (int num = 0; num < numberOfComponents; num++)
				components.Add(reader.Create<Component>());

			bool isBasedOnSomething = reader.ReadBoolean();
			if (isBasedOnSomething)
				BasedOn = reader.Create<Entity>();
		}

		private readonly List<Component> components = new List<Component>();
		public List<Component> Components
		{
			get { return components; }
		}

		public void SaveData(BinaryWriter writer)
		{
			writer.Write(Name);
			writer.Write(components.Count);
			foreach (var component in components)
				component.Save(writer);

			writer.Write(BasedOn != null);
			if (BasedOn != null)
				BasedOn.Save(writer);
		}

		public static bool operator !=(Entity entity1, Entity entity2)
		{
			return (object)entity1 == null ? (object)entity2 != null : !entity1.Equals(entity2);
		}

		public static bool operator ==(Entity entity1, Entity entity2)
		{
			return (object)entity1 == null ? (object)entity2 == null : entity1.Equals(entity2);
		}

		public bool Equals(Entity other)
		{
			return (object)other != null && Name == other.Name && BasedOn == other.BasedOn;
		}

		public override bool Equals(object other)
		{
			return other is Entity && Equals((Entity)other);
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public override string ToString()
		{
			return "Entity: " + Name + ", Components=" + Components.Count + ", BasedOn=" + BasedOn;
		}
	}
}