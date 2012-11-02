using System;
using System.Collections.Generic;
using System.Linq;
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
		public AutofacResolver Init<AppEntryRunner>()
			where AppEntryRunner : Runner
		{
			if (!typeof(AppEntryRunner).IsInterface)
				RegisterSingleton<AppEntryRunner>();
			Resolve<AppEntryRunner>();
			return this;
		}

		public AutofacResolver Init<FirstClass>(Action<FirstClass> initCode)
		{
			Register<FirstClass>();
			initCode(Resolve<FirstClass>());
			return this;
		}

		public AutofacResolver Init<FirstClass, SecondClass>(Action<FirstClass, SecondClass> initCode)
		{
			Register<FirstClass>();
			Register<SecondClass>();
			initCode(Resolve<FirstClass>(), Resolve<SecondClass>());
			return this;
		}

		public AutofacResolver Init<FirstClass, SecondClass, ThirdClass>(
			Action<FirstClass, SecondClass, ThirdClass> initCode)
		{
			Register<FirstClass>();
			Register<SecondClass>();
			Register<ThirdClass>();
			initCode(Resolve<FirstClass>(), Resolve<SecondClass>(), Resolve<ThirdClass>());
			return this;
		}

		public virtual void Run(Action runCode = null)
		{
			var window = Resolve<Window>();
			do
				ExecuteRunnersLoopAndPresenters(runCode);
			while (!window.IsClosing);
		}

		private void ExecuteRunnersLoopAndPresenters(Action runCode)
		{
			foreach (Runner runner in runners)
				runner.Run();

			if (runCode != null)
				runCode();

			foreach (Presenter presenter in presenters)
				presenter.Present();
		}

		private void Register<T>()
		{
			if (alreadyRegisteredTypes.Contains(typeof(T)))
				return;

			RegisterBaseTypes<T>(RegisterType(typeof(T)));
		}

		protected readonly List<Type> alreadyRegisteredTypes = new List<Type>();

		protected void RegisterSingleton<T>()
		{
			RegisterBaseTypes<T>(RegisterType(typeof(T)).InstancePerLifetimeScope());
		}

		private IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>
			RegisterType(Type t)
		{
			alreadyRegisteredTypes.Add(t);
			return builder.RegisterType(t).AsSelf().OnActivating(ActivatingInstance());
		}

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
			return e => AddRunnerAndPresenter(e.Component.Services, e.Instance);
		}

		private void AddRunnerAndPresenter<T>(IEnumerable<Service> services, T instance)
		{
			foreach (Service service in services)
			{
				if (service.ToString() == typeof(Runner).ToString())
					runners.Add(instance as Runner);
				if (service.ToString() == typeof(Presenter).ToString())
					presenters.Add(instance as Presenter);
			}
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

		public BaseType Resolve<BaseType>()
		{
			MakeSureContainerIsInitialized();
			return (BaseType)container.Resolve(typeof(BaseType));
		}

		protected virtual void MakeSureContainerIsInitialized()
		{
			if (IsAlreadyInitialized)
				return;

			RegisterInstance(this);
			RegisterAllTypesFromExecutable();
			container = builder.Build();
		}

		protected bool IsAlreadyInitialized
		{
			get { return container != null; }
		}

		private IContainer container;

		public void RegisterAllUnknownTypesAutomatically()
		{
			if (IsAlreadyInitialized)
				throw new RegisterCallsMustBeBeforeInit();
			builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
		}

		public class RegisterCallsMustBeBeforeInit : Exception { }

		public override void Dispose()
		{
			container.Dispose();
			builder = new ContainerBuilder();
			foreach (var instance in resolvedInstances.OfType<IDisposable>().Reverse())
				instance.Dispose();
			base.Dispose();
		}

		private void RegisterAllTypesFromExecutable()
		{
			var exeAssembly = Assembly.GetEntryAssembly();
			if (exeAssembly != null)
				//ncrunch: no coverage start
				foreach (Type t in exeAssembly.GetTypes())
					if (!alreadyRegisteredTypes.Contains(t) && typeof(Runner).IsAssignableFrom(t))
						builder.RegisterType(t)
						       .AsSelf()
						       .OnActivating(ActivatingInstance())
						       .InstancePerLifetimeScope();
		}
	}
}