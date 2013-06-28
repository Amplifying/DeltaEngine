using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DeltaEngine.Entities
{
	/// <summary>
	/// Each entity has a name, unique components for data and handlers for logic attached to them.
	/// Entities are used for rendering, game objects, ui, physics, etc.
	/// </summary>
	public abstract class Entity
	{
		/// <summary>
		/// Entities start out active and are automatically added to the EntitySystem. Call IsActive to
		/// activate or deactivate one. To disable one handler or component use <see cref="Stop{T}"/>.
		/// </summary>
		protected Entity()
		{
			IsActive = true;
		}

		public bool IsActive
		{
			get { return isActive; }
			set
			{
				if (isActive != value)
					if (value)
						Activate();
					else
						Inactivate();

				foreach (Entity component in components.OfType<Entity>())
					component.IsActive = value;
			}
		}

		private bool isActive;

		private void Activate()
		{
			isActive = true;
			isActiveChanged = true;
			if (EntitySystem.HasCurrent)
				AddToEntitySystem();
				
			if (Activated != null)
				Activated();
		}

		internal bool isActiveChanged;
		public event Action Activated;

		private void AddToEntitySystem()
		{
			EntitySystem.Current.Add(this);
			foreach (string tag in Tags)
				EntitySystem.Current.AddTag(this, tag);
		}

		private void Inactivate()
		{
			isActive = false;
			isActiveChanged = true;
			if (EntitySystem.HasCurrent)
				RemoveFromEntitySystem();

			if (Inactivated != null)
				Inactivated();
		}

		public event Action Inactivated;

		private void RemoveFromEntitySystem()
		{
			EntitySystem.Current.Remove(this);
			foreach (string tag in Tags)
				EntitySystem.Current.RemoveTag(this, tag);
		}

		public T Get<T>()
		{
			foreach (T component in components.OfType<T>())
				return component;

			throw new ComponentNotFound(typeof(T));
		}

		public class ComponentNotFound : Exception
		{
			public ComponentNotFound(Type component)
				: base(component.ToString()) {}
		}

		public T GetWithDefault<T>(T defaultValue)
		{
			foreach (T component in components.OfType<T>())
				return component;

			return defaultValue;
		}

		public T GetOrCreate<T>() where T : new()
		{
			foreach (T component in components.OfType<T>())
				return component;

			var newT = new T();
			Add(newT);
			return newT;
		}

		public bool Contains<T>()
		{
			return components.OfType<T>().Any();
		}

		public bool Contains<T1, T2>()
		{
			return Contains<T1>() && Contains<T2>();
		}

		public bool Contains<T1, T2, T3>()
		{
			return Contains<T1>() && Contains<T2>() && Contains<T3>();
		}

		public void Set<T>(T component)
		{
			ThrowExceptionIfNullOrAHandler(component);
			for (int index = 0; index < components.Count; index++)
				if (components[index] is T)
				{
					components[index] = component;
					return;
				}

			components.Add(component);
		}

		private static void ThrowExceptionIfNullOrAHandler<T>(T component)
		{
			if (component == null)
				throw new ArgumentNullException();

			if (component is Handler)
				throw new InstantiatedHandlerAddedToEntity();
		}

		public Entity Add<T>(T component)
		{
			ThrowExceptionIfNullOrAHandler(component);
			if (Contains<T>())
				throw new ComponentOfTheSameTypeAddedMoreThanOnce();

			components.Add(component);
			return this;
		}

		public class InstantiatedHandlerAddedToEntity : Exception {}

		public class ComponentOfTheSameTypeAddedMoreThanOnce : Exception {}

		internal readonly List<object> components = new List<object>();

		public Entity Start<T>()
		{
			if (!handlerTypesToAdd.Contains(typeof(T)))
				handlerTypesToAdd.Add(typeof(T));

			return this;
		}

		internal readonly List<Type> handlerTypesToAdd = new List<Type>();

		public Entity Start<T1, T2>() where T1 : Handler where T2 : Handler
		{
			Start<T1>();
			Start<T2>();
			return this;
		}

		public Entity Start<T1, T2, T3>() where T1 : Handler where T2 : Handler where T3 : Handler
		{
			Start<T1>();
			Start<T2>();
			Start<T3>();
			return this;
		}

		internal void AddHandler(Handler handler)
		{
			for (int index = 0; index < activeHandlers.Count; index++)
			{
				if (activeHandlers[index].Priority <= handler.Priority)
					continue;

				activeHandlers.Insert(index, handler);
				return;
			}
			activeHandlers.Add(handler);
		}

		internal readonly List<Handler> activeHandlers = new List<Handler>();

		public Entity Stop<T>() where T : Handler
		{
			RemoveHandler<T>();
			return this;
		}

		private void RemoveHandler<T>()
		{
			if (handlerTypesToAdd.Contains(typeof(T)))
				handlerTypesToAdd.Remove(typeof(T));
			else
				handlerTypesToRemove.Add(typeof(T));
		}

		internal readonly List<Type> handlerTypesToRemove = new List<Type>();

		public Entity Remove<T>()
		{
			if (typeof(Handler).IsAssignableFrom(typeof(T)))
				RemoveHandler<T>();
			else
				components.RemoveAll(c => c is T);

			return this;
		}

		public int NumberOfComponents
		{
			get { return components.Count; }
		}

		public Entity AddTrigger(Trigger trigger)
		{
			Start<CheckTriggers>();
			GetOrCreate<List<Trigger>>().Add(trigger);
			return this;
		}

		public Entity RemoveTrigger(Trigger trigger)
		{
			if (Contains<List<Trigger>>())
				Get<List<Trigger>>().Remove(trigger);

			return this;
		}

		public void MessageAllListeners(object message)
		{
			foreach (var listener in activeHandlers.OfType<EventListener>())
				listener.ReceiveMessage(this, message);

			if (Messaged != null)
				Messaged(message);
		}

		public event Action<object> Messaged;

		public void AddTag(string tag)
		{
			if (Tags.Contains(tag))
				return;

			EntitySystem.Current.AddTag(this, tag);
			Tags.Add(tag);
		}

		internal readonly List<string> Tags = new List<string>();

		public void RemoveTag(string tag)
		{
			EntitySystem.Current.RemoveTag(this, tag);
			Tags.Remove(tag);
		}

		public void ClearTags()
		{
			foreach (string tag in Tags)
				EntitySystem.Current.RemoveTag(this, tag);

			Tags.Clear();
		}

		public bool ContainsTag(string tag)
		{
			return Tags.Contains(tag);
		}

		public bool HandlersChanged
		{
			get { return handlerTypesToAdd.Count > 0 || handlerTypesToRemove.Count > 0 || isActiveChanged; }
		}

		public override string ToString()
		{
			return (IsActive ? "" : "<Inactive> ") + GetType().Name +
				(Tags.Count > 0 ? " Tags=" + string.Join(",", Tags) : "") +
				(components.Count > 0 ? ": " + GetTypesText(components) : "") +
				(activeHandlers.Count + handlerTypesToAdd.Count == 0
					? "" : " [" + GetTypesText(activeHandlers) + GetTypesText(handlerTypesToAdd) + "]");
		}

		private static string GetTypesText<T>(IEnumerable<T> typesList)
		{
			if (typeof(T) == typeof(Type))
				return typesList.Aggregate("",
					(text, component) => (text.Length > 0 ? text + ", " : "") + GetTypeName(component));

			return typesList.Aggregate("",
				(text, instance) => (text.Length > 0 ? text + ", " : "") + GetTypeName(instance.GetType()));
		}

		private static string GetTypeName<T>(T component)
		{
			var type = (component as Type);
			if (typeof(IList).IsAssignableFrom(type) && !type.IsArray)
				return type.Name.Replace("`1", "") + "<" + GetTypesText(type.GetGenericArguments()) + ">";

			return type.Name;
		}
	}
}