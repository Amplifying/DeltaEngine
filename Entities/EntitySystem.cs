using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;

namespace DeltaEngine.Entities
{
	/// <summary>
	/// Keeps a list of all active entities and manages all entity handlers created via resolvers.
	/// </summary>
	public class EntitySystem : Runner
	{
		public EntitySystem(HandlerResolver handlerResolver)
		{
			this.handlerResolver = handlerResolver;
		}

		private readonly HandlerResolver handlerResolver;

		public static bool HasCurrent
		{
			get { return ThreadStaticEntitySystem.HasCurrent; }
		}

		public static EntitySystem Current
		{
			get { return ThreadStaticEntitySystem.Current; }
		}

		private static readonly ThreadStatic<EntitySystem> ThreadStaticEntitySystem =
			new ThreadStatic<EntitySystem>();

		public static IDisposable Use(EntitySystem entitySystem)
		{
			return ThreadStaticEntitySystem.Use(entitySystem);
		}

		internal void Add(Entity entity)
		{
			if (rememberToRemove.Contains(entity))
			{
				rememberToRemove.Remove(entity);
				return;
			}

			if (WasEntityAlreadyAddedBefore(entity))
				throw new EntityAlreadyAdded();

			rememberToAdd.Add(entity);
		}

		private readonly List<Entity> rememberToRemove = new List<Entity>();
		private readonly List<Entity> rememberToAdd = new List<Entity>();

		public class EntityAlreadyAdded : Exception {}

		private bool WasEntityAlreadyAddedBefore(Entity entity)
		{
			if (entitiesWithoutHandlers.Contains(entity) || rememberToAdd.Contains(entity))
				return true;

			return
				entity.activeHandlers.Any(
					handler => FindHandlerWithEntities(handler).entities.Contains(entity));
		}

		private readonly List<Entity> entitiesWithoutHandlers = new List<Entity>();

		private HandlerWithEntities FindHandlerWithEntities(Handler searchHandler)
		{
			return handlersWithAffectedEntities.FirstOrDefault(h => h.handler == searchHandler);
		}

		internal void Remove(Entity entity)
		{
			if (rememberToAdd.Contains(entity))
				rememberToAdd.Remove(entity);
			else
				rememberToRemove.Add(entity);
		}

		public void Clear()
		{
			rememberToAdd.Clear();
			foreach (var entity in entitiesWithoutHandlers)
				rememberToRemove.Add(entity);

			foreach (var pair in handlersWithAffectedEntities)
				foreach (var entity in pair.entities)
					rememberToRemove.Add(entity);
		}

		private readonly List<HandlerWithEntities> handlersWithAffectedEntities =
			new List<HandlerWithEntities>();

		private class HandlerWithEntities
		{
			public HandlerWithEntities(Handler handler)
			{
				this.handler = handler;
			}

			public readonly Handler handler;
			public readonly List<Entity> entities = new List<Entity>();
		}

		internal void AddTag(Entity entity, string tag)
		{
			List<Entity> entitiesWithTag;
			if (entityTags.TryGetValue(tag, out entitiesWithTag))
				entitiesWithTag.Add(entity);
			else
				entityTags.Add(tag, new List<Entity> { entity });
		}

		private readonly Dictionary<string, List<Entity>> entityTags =
			new Dictionary<string, List<Entity>>();

		internal void RemoveTag(Entity entity, string tag)
		{
			List<Entity> entitiesWithTag;
			if (entityTags.TryGetValue(tag, out entitiesWithTag))
				entitiesWithTag.Remove(entity);
		}

		public List<Entity> GetEntitiesWithTag(string searchTag)
		{
			List<Entity> entitiesWithTag;
			return entityTags.TryGetValue(searchTag, out entitiesWithTag)
				? entitiesWithTag : new List<Entity>();
		}

		public List<Entity> GetEntitiesByHandler(Handler handler)
		{
			HandlerWithEntities handlerWithEntities = FindHandlerWithEntities(handler);
			return handlerWithEntities == null ? new List<Entity>() : handlerWithEntities.entities;
		}

		public List<T> GetEntitiesOfType<T>()
		{
			return GetAllEntitiesInCurrent().OfType<T>().ToList();
		}

		public int NumberOfEntities
		{
			get { return GetAllEntitiesInCurrent().Count; }
		}

		private List<Entity> GetAllEntitiesInCurrent()
		{
			var total = new List<Entity>(entitiesWithoutHandlers);
			total.AddRange(
				handlersWithAffectedEntities.SelectMany(handler => handler.entities).Distinct());
			total.AddRange(rememberToAdd);
			foreach (var entity in rememberToRemove)
				total.Remove(entity);
			return total;
		}

		public void Run()
		{
			AddRememberedEntities();
			RemoveRememberedEntities();
			UpdateHandlersIfAnythingHasChanged();
			foreach (var pair in handlersWithAffectedEntities)
				RunHandler(pair.handler, pair.entities);
		}

		private void AddRememberedEntities()
		{
			lock (rememberToAdd)
			{
				foreach (var entity in rememberToAdd)
					AddRememberedEntity(entity);

				rememberToAdd.Clear();
			}
		}

		private void AddRememberedEntity(Entity entity)
		{
			RemoveAndAddUnresolvedEntityHandlers(entity);
			if (entity.activeHandlers.Count > 0)
				AddToAllAttachedHandlers(entity);
			else
				entitiesWithoutHandlers.Add(entity);
		}

		private void RemoveAndAddUnresolvedEntityHandlers(Entity entity)
		{
			foreach (Type handlerType in entity.handlerTypesToRemove)
				RemoveHandlerAndRunStoppedHandlingCode(entity, handlerType);

			foreach (Type handlerType in entity.handlerTypesToAdd)
				AddHandlerAndRunStartedHandlingCode(entity, handlerType);

			entity.handlerTypesToAdd.Clear();
			entity.handlerTypesToRemove.Clear();
			entity.isActiveChanged = false;
		}

		private void AddHandlerAndRunStartedHandlingCode(Entity entity, Type handlerType)
		{
			Handler handler = GetHandler(handlerType);
			if (entity.activeHandlers.Contains(handler))
				return;

			var behavior = handler as Behavior<Entity>;
			if (behavior != null)
				behavior.StartedHandling(entity);

			entity.AddHandler(GetHandler(handlerType));
		}

		private void RemoveHandlerAndRunStoppedHandlingCode(Entity entity, Type handlerType)
		{
			Handler handler = GetHandler(handlerType);
			if (!entity.activeHandlers.Contains(handler))
				return;

			var behavior = handler as Behavior<Entity>;
			if (behavior != null)
				behavior.StoppedHandling(entity);

			entity.activeHandlers.Remove(handler);
		}

		public T GetHandler<T>() where T : Handler
		{
			return GetHandler(typeof(T)) as T;
		}

		private Handler GetHandler(Type handlerType)
		{
			foreach (var handler in handlersWithAffectedEntities)
				if (handler.handler.GetType() == handlerType)
					return handler.handler;

			var newHandler = handlerResolver.Resolve(handlerType);
			if (newHandler == null)
				throw new UnableToResolveEntityHandler(handlerType);

			AddHandlerSortedByPriority(new HandlerWithEntities(newHandler));
			return newHandler;
		}

		public class UnableToResolveEntityHandler : Exception
		{
			public UnableToResolveEntityHandler(Type handlerType)
				: base(handlerType.ToString()) {}
		}

		private void AddHandlerSortedByPriority(HandlerWithEntities entities)
		{
			for (int index = 0; index < handlersWithAffectedEntities.Count; index++)
			{
				if (handlersWithAffectedEntities[index].handler.Priority <= entities.handler.Priority)
					continue;
				handlersWithAffectedEntities.Insert(index, entities);
				return;
			}
			handlersWithAffectedEntities.Add(entities);
		}

		private void AddToAllAttachedHandlers(Entity entity)
		{
			foreach (var handler in entity.activeHandlers)
				FindHandlerWithEntities(handler).entities.Add(entity);
		}

		private void RemoveRememberedEntities()
		{
			lock (rememberToRemove)
			{
				foreach (var entity in rememberToRemove)
					RemoveRememberedEntity(entity);

				rememberToRemove.Clear();
			}
		}

		private void RemoveRememberedEntity(Entity entity)
		{
			entitiesWithoutHandlers.Remove(entity);
			foreach (var handler in entity.activeHandlers)
				FindHandlerWithEntities(handler).entities.Remove(entity);
		}

		private void UpdateHandlersIfAnythingHasChanged()
		{
			var entitiesWithHandlersChanged = new List<Entity>();
			entitiesWithHandlersChanged.AddRange(entitiesWithoutHandlers.Where(e => e.HandlersChanged));
			foreach (var handler in handlersWithAffectedEntities)
				entitiesWithHandlersChanged.AddRange(handler.entities.Where(e => e.HandlersChanged));

			foreach (var entity in entitiesWithHandlersChanged)
				HandlersChanged(entity);
		}

		private void HandlersChanged(Entity entity)
		{
			RemoveAndAddUnresolvedEntityHandlers(entity);
			if (entity.IsActive)
				AddMissingHandlersAndRemoveUnusedHandlers(entity);
			else
				DisableHandlersForInactiveEntity(entity);
		}

		private void AddMissingHandlersAndRemoveUnusedHandlers(Entity entity)
		{
			foreach (var pair in handlersWithAffectedEntities)
				if (entity.activeHandlers.Contains(pair.handler) && !pair.entities.Contains(entity))
					pair.entities.Add(entity);
				else if (!entity.activeHandlers.Contains(pair.handler) && pair.entities.Contains(entity))
					pair.entities.Remove(entity);
		}

		private void DisableHandlersForInactiveEntity(Entity entity)
		{
			foreach (var pair in handlersWithAffectedEntities)
				if (pair.entities.Contains(entity))
					pair.entities.Remove(entity);
		}

		private static void RunHandler(Handler handler, IEnumerable<Entity> entities)
		{
			var behavior = handler as Behavior<Entity>;
			var batchedBehavior = handler as BatchedBehavior<Entity>;
			if (behavior == null && batchedBehavior == null)
				return;

			if (behavior != null)
				RunBehavior(behavior, entities);
			else
				RunBatchedBehavior(batchedBehavior, entities);
		}

		private static void RunBehavior(Behavior<Entity> behavior, IEnumerable<Entity> entities)
		{
			entities = FilterAndSortEntities(entities, behavior.Filter, behavior.Order);
			foreach (Entity entity in entities)
				behavior.Handle(entity);
		}

		private static IEnumerable<Entity> FilterAndSortEntities(IEnumerable<Entity> entities,
			Func<Entity, bool> filter, Func<Entity, IComparable> order)
		{
			if (filter != null && order != null)
				return entities.Where(filter).OrderBy(order);

			if (filter != null)
				return entities.Where(filter);

			return order != null ? entities.OrderBy(order) : entities;
		}

		private static void RunBatchedBehavior(BatchedBehavior<Entity> batchedBehavior,
			IEnumerable<Entity> entities)
		{
			entities = FilterAndSortEntities(entities, batchedBehavior.Filter, batchedBehavior.Order);
			batchedBehavior.Handle(entities);
		}

		public void MessageAllListeners(object message)
		{
			foreach (
				var pair in handlersWithAffectedEntities.Where(pair => pair.handler is EventListener))
				((EventListener)pair.handler).ReceiveMessage(message);
		}
	}
}