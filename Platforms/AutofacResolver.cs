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

			RegisterNonConcreteBaseTypes(typeToRegister, RegisterType(typeToRegister));
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

			RegisterNonConcreteBaseTypes(typeToRegister,
				RegisterType(typeToRegister).InstancePerLifetimeScope());
		}

		private
			IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>
			RegisterType(Type t)
		{
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

		private void RegisterNonConcreteBaseTypes(Type typeToRegister,
			IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>
				registration)
		{
			foreach (var type in typeToRegister.GetInterfaces())
				AddRegisteredType(type);

			registration.AsImplementedInterfaces();
			var baseType = typeToRegister.BaseType;
			while (baseType != null && baseType != typeof(object))
			{
				if (baseType.IsAbstract)
				{
					AddRegisteredType(baseType);
					registration.As(baseType);
				}
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

			ForceCoreXmlContentToBeLoaded();
			Register<FirstResolveType>();
			RegisterAllTypesFromAllAssemblies<ContentData, EntityHandler>();
		}

		private void ForceCoreXmlContentToBeLoaded()
		{
			Register<XmlContent>();
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
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var dependencyAssemblies = new List<Assembly>();
			foreach (Assembly assembly in assemblies.Where(assembly => assembly.IsAllowed()))
			{
				RegisterAllTypesInAssembly<InstanceType>(assembly, false);
				RegisterAllTypesInAssembly<SingletonType>(assembly, true);
				RegisterAllTypesInAssembly(assembly, assemblies, dependencyAssemblies);
			}
		}

		private void RegisterAllTypesInAssembly<T>(Assembly assembly, bool registerAsSingleton)
		{
			foreach (Type type in assembly.GetTypes())
				if (typeof(T).IsAssignableFrom(type) && !type.IsAbstract && !type.FullName.Contains("Mock") &&
					!IgnoreForResolverAttribute.IsTypeIgnored(type))
					if (registerAsSingleton)
						RegisterSingleton(type);
					else
						Register(type);
		}

		private void RegisterAllTypesInAssembly(Assembly assembly, Assembly[] loadedAssemblies,
			List<Assembly> dependencyAssemblies)
		{
			if (assembly.FullName.StartsWith("DeltaEngine.") && !assembly.FullName.Contains(".Scenes"))
				return;

			LoadDependentAssemblies(assembly, loadedAssemblies, dependencyAssemblies);
			var assemblyTypes = assembly.GetTypes();
			foreach (Type t in assemblyTypes)
				if (!alreadyRegisteredTypes.Contains(t))
				{
					builder.RegisterType(t).AsSelf().OnActivating(ActivatingInstance()).
									InstancePerLifetimeScope();
					AddRegisteredType(t);
				}
		}

		private void LoadDependentAssemblies(Assembly assembly, Assembly[] loadedAssemblies,
			List<Assembly> dependencyAssemblies)
		{
			foreach (var dependency in assembly.GetReferencedAssemblies())
				if (dependency.IsAllowed() &&
					loadedAssemblies.All(loaded => dependency.Name != loaded.GetName().Name) &&
					dependencyAssemblies.All(loaded => dependency.Name != loaded.GetName().Name))
				{
					var newAssembly = Assembly.Load(dependency);
					dependencyAssemblies.Add(newAssembly);
					RegisterAllTypesInAssembly(newAssembly, loadedAssemblies, dependencyAssemblies);
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
	}
}