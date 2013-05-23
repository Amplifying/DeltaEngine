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
		public EntitySystem(EntityHandlerResolver handlerResolver)
		{
			this.handlerResolver = handlerResolver;
		}

		private readonly EntityHandlerResolver handlerResolver;

		public void Add(Entity entity)
		{
			entity.IsActive = true;
			if (WasEntityAlreadyAddedBefore(entity))
				return;

			rememberToAdd.Add(entity);
		}

		private readonly List<Entity> rememberToAdd = new List<Entity>();

		public void Remove(Entity entity)
		{
			entity.IsActive = false;
			if (rememberToAdd.Contains(entity))
				rememberToAdd.Remove(entity);
			else
				rememberToRemove.Add(entity);
		}

		private readonly List<Entity> rememberToRemove = new List<Entity>();

		private bool WasEntityAlreadyAddedBefore(Entity entity)
		{
			if (entitiesWithoutHandlers.Contains(entity))
				return true;

			foreach (var handler in entity.activeHandlers)
				if (FindHandlerWithEntities(handler).entities.Contains(entity))
					return true;

			return false;
		}

		private HandlerWithEntities FindHandlerWithEntities(EntityHandler searchHandler)
		{
			return handlersWithAffectedEntities.FirstOrDefault(h => h.handler == searchHandler);
		}

		private readonly List<HandlerWithEntities> handlersWithAffectedEntities =
			new List<HandlerWithEntities>();

		private class HandlerWithEntities
		{
			public HandlerWithEntities(EntityHandler handler)
			{
				this.handler = handler;
			}

			public readonly EntityHandler handler;
			public readonly List<Entity> entities = new List<Entity>();
		}

		private void RemoveAndAddUnresolvedEntityHandlers(Entity entity)
		{
			foreach (Type handlerType in entity.handlerTypesToRemove)
				entity.activeHandlers.Remove(GetHandler(handlerType));

			foreach (Type handlerType in entity.handlerTypesToAdd)
				entity.AddHandler(GetHandler(handlerType));

			entity.handlerTypesToAdd.Clear();
			entity.handlerTypesToRemove.Clear();
			entity.isActiveChanged = false;
		}

		private EntityHandler GetHandler(Type handlerType)
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

		public T GetHandler<T>() where T : class, EntityHandler
		{
			return GetHandler(typeof(T)) as T;
		}

		private void AddToAllAttachedHandlers(Entity entity)
		{
			foreach (var handler in entity.activeHandlers)
				FindHandlerWithEntities(handler).entities.Add(entity);
		}

		private readonly List<Entity> entitiesWithoutHandlers = new List<Entity>();

		public List<Entity> GetEntitiesWithTag(string searchTag)
		{
			foundEntities = new List<Entity>();
			foreach (var handler in handlersWithAffectedEntities)
				SearchEntitiesWithTag(handler.entities, searchTag);

			SearchEntitiesWithTag(entitiesWithoutHandlers, searchTag);
			SearchEntitiesWithTag(rememberToAdd, searchTag);
			return foundEntities;
		}

		private List<Entity> foundEntities;

		private void SearchEntitiesWithTag(IEnumerable<Entity> entities, string searchTag)
		{
			foreach (var entity in entities.Where(entity => entity.Tag == searchTag))
				if (!foundEntities.Contains(entity))
					foundEntities.Add(entity);
		}

		public int NumberOfEntities
		{
			get
			{
				var total = new List<Entity>(entitiesWithoutHandlers);
				foreach (var handler in handlersWithAffectedEntities)
					foreach (var entity in handler.entities)
						if (!total.Contains(entity))
							total.Add(entity);
				total.AddRange(rememberToAdd);
				foreach (var entity in rememberToRemove)
					total.Remove(entity);
				return total.Count;
			}
		}

		public void Run()
		{
			AddRememberedEntities();
			RemoveRememberedEntities();
			UpdateHandlersIfAnythingHasChanged();
			foreach (var pair in handlersWithAffectedEntities)
				pair.handler.Handle(pair.entities);
		}

		private void AddRememberedEntities()
		{
			foreach (var entity in rememberToAdd)
				AddRememberedEntity(entity);
			rememberToAdd.Clear();
		}

		private void AddRememberedEntity(Entity entity)
		{
			RemoveAndAddUnresolvedEntityHandlers(entity);
			if (entity.activeHandlers.Count > 0)
				AddToAllAttachedHandlers(entity);
			else
				entitiesWithoutHandlers.Add(entity);
		}

		private void RemoveRememberedEntities()
		{
			foreach (var entity in rememberToRemove)
				RemoveRememberedEntity(entity);
			rememberToRemove.Clear();
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
	}
}