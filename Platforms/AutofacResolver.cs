using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Features.ResolveAnything;
using DeltaEngine.Core;

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
			if (alreadyRegisteredTypes.Contains(typeof(T)))
				return;

			if (IsAlreadyInitialized)
				throw new UnableToRegisterMoreTypesAppAlreadyStarted();

			RegisterBaseTypes<T>(RegisterType(typeof(T)));
		}

		protected readonly List<Type> alreadyRegisteredTypes = new List<Type>();

		internal class UnableToRegisterMoreTypesAppAlreadyStarted : Exception {}

		public void RegisterSingleton<T>()
		{
			if (alreadyRegisteredTypes.Contains(typeof(T)))
				return;

			if (IsAlreadyInitialized)
				throw new UnableToRegisterMoreTypesAppAlreadyStarted();

			RegisterBaseTypes<T>(RegisterType(typeof(T)).InstancePerLifetimeScope());
		}

		private
			IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>
			RegisterType(Type t)
		{
			if (assemblyOfFirstNonDeltaEngineType == null &&
				!t.Assembly.FullName.Contains("DeltaEngine."))
				assemblyOfFirstNonDeltaEngineType = t.Assembly;

			alreadyRegisteredTypes.Add(t);
			return builder.RegisterType(t).AsSelf().OnActivating(ActivatingInstance());
		}

		private Assembly assemblyOfFirstNonDeltaEngineType;
		private ContainerBuilder builder = new ContainerBuilder();

		protected void RegisterInstance(object instance)
		{
			var registration =
				builder.RegisterInstance(instance).SingleInstance().AsImplementedInterfaces();
			alreadyRegisteredTypes.Add(instance.GetType());
			RegisterAllBaseTypes(instance.GetType().BaseType, registration);
		}

		private void RegisterAllBaseTypes(Type baseType,
			IRegistrationBuilder<object, SimpleActivatorData, SingleRegistrationStyle> registration)
		{
			while (baseType != null && baseType != typeof(object))
			{
				alreadyRegisteredTypes.Add(baseType);
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

		private void RegisterBaseTypes<T>(
			IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>
				registration)
		{
			alreadyRegisteredTypes.AddRange(typeof(T).GetInterfaces());
			registration.AsImplementedInterfaces();
			var baseType = typeof(T).BaseType;
			while (baseType != null && baseType != typeof(object))
			{
				alreadyRegisteredTypes.Add(baseType);
				registration.As(baseType);
				baseType = baseType.BaseType;
			}
		}

		public override BaseType Resolve<BaseType>(object customParameter = null)
		{
			if (IsAlreadyInitialized == false)
				Register<BaseType>();

			MakeSureContainerIsInitialized();
			if (customParameter == null)
				return (BaseType)container.Resolve(typeof(BaseType));

			Parameter autofacParameter = new TypedParameter(customParameter.GetType(), customParameter);
			return (BaseType)container.Resolve(typeof(BaseType), new[] { autofacParameter });
		}

		protected virtual void MakeSureContainerIsInitialized()
		{
			if (IsAlreadyInitialized)
				return; //ncrunch: no coverage

			RegisterInstance(this);
			RegisterAllTypesFromExecutable();
			container = builder.Build();
		}

		protected bool IsAlreadyInitialized
		{
			get { return container != null; }
		}

		private IContainer container;

		protected override object Resolve(Type baseType)
		{
			MakeSureContainerIsInitialized();
			return container.Resolve(baseType);
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
					builder.RegisterType(t).AsSelf().OnActivating(ActivatingInstance()).
					        InstancePerLifetimeScope();
		}
	}
}