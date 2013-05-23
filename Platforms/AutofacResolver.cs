using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Features.ResolveAnything;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Core.Xml;
using DeltaEngine.Entities;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Each configuration needs to register concrete types (e.g. graphics). All types derived from
	/// Runner or Presenter are automatically run in AppRunner.
	/// </summary>
	public abstract class AutofacResolver : Resolver
	{
		public void Register<T>()
		{
			Register(typeof(T));
		}

		public void Register(Type typeToRegister)
		{
			if (alreadyRegisteredTypes.Contains(typeToRegister))
				return;

			if (IsAlreadyInitialized)
				throw new UnableToRegisterMoreTypesAppAlreadyStarted();

			RegisterBaseTypes(typeToRegister, RegisterType(typeToRegister));
		}

		protected readonly List<Type> alreadyRegisteredTypes = new List<Type>();

		internal class UnableToRegisterMoreTypesAppAlreadyStarted : Exception {}

		public void RegisterSingleton<T>()
		{
			RegisterSingleton(typeof(T));
		}

		public void RegisterSingleton(Type typeToRegister)
		{
			if (alreadyRegisteredTypes.Contains(typeToRegister))
				return;

			if (IsAlreadyInitialized)
				throw new UnableToRegisterMoreTypesAppAlreadyStarted();

			RegisterBaseTypes(typeToRegister, RegisterType(typeToRegister).InstancePerLifetimeScope());
		}

		private
			IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>
			RegisterType(Type t)
		{
			if (assemblyOfFirstNonDeltaEngineType == null &&
				!t.Assembly.FullName.Contains("DeltaEngine."))
				assemblyOfFirstNonDeltaEngineType = t.Assembly; // ncrunch: no coverage

			AddRegisteredType(t);
			return builder.RegisterType(t).AsSelf().OnActivating(ActivatingInstance());
		}

		private void AddRegisteredType(Type t)
		{
			if (!alreadyRegisteredTypes.Contains(t))
			{
				alreadyRegisteredTypes.Add(t);
				return;
			}

			if (ExceptionExtensions.IsDebugMode && !t.IsInterface && !t.IsAbstract)
				Console.WriteLine("Warning: Type " + t + " already exists in alreadyRegisteredTypes");
		}

		private Assembly assemblyOfFirstNonDeltaEngineType;
		private ContainerBuilder builder = new ContainerBuilder();

		protected void RegisterInstance(object instance)
		{
			var registration =
				builder.RegisterInstance(instance).SingleInstance().AsSelf().AsImplementedInterfaces();
			RegisterInstanceAsRunnerOrPresenterIfPossible(instance);
			AddRegisteredType(instance.GetType());
			RegisterAllBaseTypes(instance.GetType().BaseType, registration);
		}

		private void RegisterAllBaseTypes(Type baseType,
			IRegistrationBuilder<object, SimpleActivatorData, SingleRegistrationStyle> registration)
		{
			while (baseType != null && baseType != typeof(object))
			{
				AddRegisteredType(baseType);
				registration.As(baseType);
				baseType = baseType.BaseType;
			}
		}

		private Action<IActivatingEventArgs<object>> ActivatingInstance()
		{
			return e => ProcessActivatedInstance(e.Instance);
		}

		private void ProcessActivatedInstance<T>(T instance)
		{
			container.InjectUnsetProperties(instance);
			RegisterInstanceAsRunnerOrPresenterIfPossible(instance);
			resolvedInstances.Add(instance);
		}

		private readonly List<object> resolvedInstances = new List<object>();

		private void RegisterBaseTypes(Type typeToRegister,
			IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>
				registration)
		{
			foreach (var type in typeToRegister.GetInterfaces())
				AddRegisteredType(type);
			registration.AsImplementedInterfaces();
			var baseType = typeToRegister.BaseType;
			while (baseType != null && baseType != typeof(object))
			{
				AddRegisteredType(baseType);
				registration.As(baseType);
				baseType = baseType.BaseType;
			}
		}

		internal override BaseType Resolve<BaseType>()
		{
			RegisterUnknownTypes<BaseType>();
			MakeSureContainerIsInitialized();
			return (BaseType)container.Resolve(typeof(BaseType));
		}

		private void RegisterUnknownTypes<FirstResolveType>()
		{
			if (IsAlreadyInitialized)
				return;

			RegisterAllTypesFromAllAssemblies<ContentData, EntityHandler>();
			// This line is required to force Core.Xml Assembly to load to register XmlContent
			Register<XmlContent>();
			RegisterInstance(new EntitySystem(new AutofacEntityResolver(this)));
			RegisterAllTypesFromExecutable();
			Register<FirstResolveType>();
		}

		protected virtual void MakeSureContainerIsInitialized()
		{
			if (IsAlreadyInitialized)
				return; //ncrunch: no coverage

			container = builder.Build();
		}

		protected bool IsAlreadyInitialized
		{
			get { return container != null; }
		}

		private IContainer container;

		protected class AutofacContentDataResolver : ContentDataResolver
		{
			public AutofacContentDataResolver(Resolver resolver)
			{
				this.resolver = resolver;
			}

			private readonly Resolver resolver;

			public ContentData Resolve(Type contentType, string contentName)
			{
				return resolver.Resolve(contentType, contentName) as ContentData;
			}
		}

		private void RegisterAllTypesFromAllAssemblies<InstanceType, SingletonType>()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies.Where(assembly => assembly.IsAllowed()))
			{
				RegisterAllTypesInAssembly<InstanceType>(assembly, false);
				RegisterAllTypesInAssembly<SingletonType>(assembly, true);
			}
		}

		private void RegisterAllTypesInAssembly<T>(Assembly exeAssembly, bool registerAsSingleton)
		{
			foreach (Type type in exeAssembly.GetTypes())
				if (typeof(T).IsAssignableFrom(type) && !type.IsAbstract &&
					!type.FullName.Contains("Mock") && !IgnoreForResolverAttribute.IsTypeIgnored(type))
					if (registerAsSingleton)
						RegisterSingleton(type);
					else
						Register(type);
		}

		private class AutofacEntityResolver : EntityHandlerResolver
		{
			public AutofacEntityResolver(Resolver resolver)
			{
				this.resolver = resolver;
			}

			private readonly Resolver resolver;

			public EntityHandler Resolve(Type handlerType)
			{
				return resolver.Resolve(handlerType) as EntityHandler;
			}
		}

		internal override object Resolve(Type baseType, object customParameter = null)
		{
			MakeSureContainerIsInitialized();
			if (customParameter == null)
				return container.Resolve(baseType);
			
			Parameter autofacParameter = new TypedParameter(customParameter.GetType(), customParameter);
			return container.Resolve(baseType, new[] { autofacParameter });
		}

		public void RegisterAllUnknownTypesAutomatically()
		{
			if (IsAlreadyInitialized)
				throw new RegisterCallsMustBeBeforeInit();
			builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
		}

		public class RegisterCallsMustBeBeforeInit : Exception {}

		public override void Dispose()
		{
			if (IsAlreadyInitialized)
				DisposeContainerOnlyOnce();
		}

		private void DisposeContainerOnlyOnce()
		{
			var remContainerToDispose = container;
			container = null;
			remContainerToDispose.Dispose();
			builder = new ContainerBuilder();
		}

		//ncrunch: no coverage start
		private void RegisterAllTypesFromExecutable()
		{
			var exeAssembly = Assembly.GetEntryAssembly();
			if (exeAssembly != null)
				RegisterAllTypesInAssembly(exeAssembly);
			if (assemblyOfFirstNonDeltaEngineType != null &&
				assemblyOfFirstNonDeltaEngineType != exeAssembly)
				RegisterAllTypesInAssembly(assemblyOfFirstNonDeltaEngineType);
		}

		private void RegisterAllTypesInAssembly(Assembly exeAssembly)
		{
			foreach (Type t in exeAssembly.GetTypes())
				if (!alreadyRegisteredTypes.Contains(t))
				{
					builder.RegisterType(t).AsSelf().OnActivating(ActivatingInstance()).
									InstancePerLifetimeScope();
					AddRegisteredType(t);
				}
		}
	}
}