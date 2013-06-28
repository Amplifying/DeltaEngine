using System;

namespace DeltaEngine.Entities
{
	/// <summary>
	/// Abstract factory to provide access to create entity handlers on demand via the active resolver
	/// </summary>
	public interface HandlerResolver
	{
		Handler Resolve(Type handlerType);
	}
}