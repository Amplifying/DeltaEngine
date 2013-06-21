using System;
using DeltaEngine.Core;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	/// <summary>
	/// Base class to run any test as an automated unit test, integration, visual and approval test.
	/// http://DeltaEngine.net/About/CodingStyle#Tests
	/// </summary>
	[TestFixture]
	public abstract class TestWithMockResolver : TestApprovalImages
	{
		public void Start<AppEntryRunner>(Type resolverType, int instancesToCreate = 1)
		{
			if (IgnoreSlowTestIfStartedViaNCrunchOrNunitConsole(resolverType))
				return; //ncrunch: no coverage

			using (var resolver = CreateResolver(resolverType))
				resolver.Start<AppEntryRunner>(instancesToCreate);
		}

		private AutofacStarter CreateResolver(Type resolverType)
		{
			if (resolverType != typeof(MockResolver)) //ncrunch: no coverage start 
			{
				mockResolver = null;
				return (AutofacStarter)Activator.CreateInstance(resolverType);
			} //ncrunch: no coverage end

			mockResolver = new MockResolver();
			return mockResolver.resolver;
		}

		protected MockResolver mockResolver;

		protected void Start(Type resolverType, Action initCode, Action runCode = null)
		{
			if (IgnoreSlowTestIfStartedViaNCrunchOrNunitConsole(resolverType))
				return; //ncrunch: no coverage

			using (var resolver = CreateResolver(resolverType))
			{
				// If we don't resolve anything at all pre-start, no delta engine classes end up 
				// getting registered - which means when, later, EntitySystem tries to resolve a handler 
				// it fails as unregistered
				resolver.Resolve<PseudoRandom>();
				resolver.Start(initCode, AddApprovalCheckToRunCode(resolver, runCode));
			}

			CheckApprovalTestResult();
		}

		protected void Start<First>(Type resolverType, Action<First> initCode, Action runCode = null)
		{
			if (IgnoreSlowTestIfStartedViaNCrunchOrNunitConsole(resolverType))
				return; //ncrunch: no coverage

			using (var resolver = CreateResolver(resolverType))
				resolver.Start(initCode, AddApprovalCheckToRunCode(resolver, runCode));

			CheckApprovalTestResult();
		}

		protected void Start<First, Second>(Type resolverType, Action<First> initCode,
			Action<Second> runCode)
		{
			if (IgnoreSlowTestIfStartedViaNCrunchOrNunitConsole(resolverType))
				return; //ncrunch: no coverage

			//// ReSharper disable AccessToDisposedClosure
			using (var resolver = CreateResolver(resolverType))
			{
				resolver.RegisterSingleton<Second>();
				resolver.Start(initCode,
					AddApprovalCheckToRunCode(resolver, () => runCode(resolver.Resolve<Second>())));
			}
			CheckApprovalTestResult();
		}

		protected void Start<First, Second>(Type resolverType, Action<First, Second> initCode,
			Action runCode = null)
		{
			if (IgnoreSlowTestIfStartedViaNCrunchOrNunitConsole(resolverType))
				return; //ncrunch: no coverage

			using (var resolver = CreateResolver(resolverType))
				resolver.Start(initCode, AddApprovalCheckToRunCode(resolver, runCode));

			CheckApprovalTestResult();
		}

		protected void Start<First, Second, Third>(Type resolverType,
			Action<First, Second, Third> initCode, Action runCode = null)
		{
			if (IgnoreSlowTestIfStartedViaNCrunchOrNunitConsole(resolverType))
				return; //ncrunch: no coverage

			using (var resolver = CreateResolver(resolverType))
				resolver.Start(initCode, AddApprovalCheckToRunCode(resolver, runCode));

			CheckApprovalTestResult();
		}

		protected void Start<First, Second, Third, Fourth>(Type resolverType,
			Action<First, Second, Third, Fourth> initCode, Action runCode = null)
		{
			if (IgnoreSlowTestIfStartedViaNCrunchOrNunitConsole(resolverType))
				return; //ncrunch: no coverage

			using (var resolver = CreateResolver(resolverType))
				resolver.Start(initCode, AddApprovalCheckToRunCode(resolver, runCode));

			CheckApprovalTestResult();
		}

		protected void Start<First, Second, Third, Fourth, Fifth>(Type resolverType,
			Action<First, Second, Third, Fourth, Fifth> initCode, Action runCode = null)
		{
			if (IgnoreSlowTestIfStartedViaNCrunchOrNunitConsole(resolverType))
				return; //ncrunch: no coverage

			using (var resolver = CreateResolver(resolverType))
				resolver.Start(initCode, AddApprovalCheckToRunCode(resolver, runCode));

			CheckApprovalTestResult();
		}

		protected void Start<First>(Type resolverType, Action initCode, Action<First> runCode)
		{
			if (IgnoreSlowTestIfStartedViaNCrunchOrNunitConsole(resolverType))
				return; //ncrunch: no coverage

			using (var resolver = CreateResolver(resolverType))
			{
				resolver.RegisterSingleton<First>();
				resolver.Start(initCode,
					AddApprovalCheckToRunCode(resolver, () => runCode(resolver.Resolve<First>())));
			}
			CheckApprovalTestResult();
		}

		protected void Start<First, Second>(Type resolverType, Action initCode,
			Action<First, Second> runCode)
		{
			if (IgnoreSlowTestIfStartedViaNCrunchOrNunitConsole(resolverType))
				return; //ncrunch: no coverage

			using (var resolver = CreateResolver(resolverType))
			{
				resolver.RegisterSingleton<First>();
				resolver.RegisterSingleton<Second>();
				resolver.Start(initCode,
					AddApprovalCheckToRunCode(resolver,
						() => runCode(resolver.Resolve<First>(), resolver.Resolve<Second>())));
			}
			CheckApprovalTestResult();
		}

		protected void Start<First, Second, Third>(Type resolverType, Action<First, Second> initCode,
			Action<Third> runCode)
		{
			if (IgnoreSlowTestIfStartedViaNCrunchOrNunitConsole(resolverType))
				return; //ncrunch: no coverage

			using (var resolver = CreateResolver(resolverType))
			{
				resolver.RegisterSingleton<Third>();
				resolver.Start(initCode,
					AddApprovalCheckToRunCode(resolver, () => runCode(resolver.Resolve<Third>())));
			}
			CheckApprovalTestResult();
		}

		protected void Start<First, Second, Third>(Type resolverType, Action<First> initCode,
			Action<Second, Third> runCode)
		{
			if (IgnoreSlowTestIfStartedViaNCrunchOrNunitConsole(resolverType))
				return; //ncrunch: no coverage

			using (var resolver = CreateResolver(resolverType))
			{
				resolver.RegisterSingleton<Second>();
				resolver.RegisterSingleton<Third>();
				resolver.Start(initCode,
					AddApprovalCheckToRunCode(resolver,
						() => runCode(resolver.Resolve<Second>(), resolver.Resolve<Third>())));
			}
			CheckApprovalTestResult();
		}

		protected void Start<First, Second, Third, Forth>(Type resolverType,
			Action<First, Second, Third> initCode, Action<Forth> runCode)
		{
			if (IgnoreSlowTestIfStartedViaNCrunchOrNunitConsole(resolverType))
				return; //ncrunch: no coverage

			using (var resolver = CreateResolver(resolverType))
			{
				resolver.RegisterSingleton<Forth>();
				resolver.Start(initCode,
					AddApprovalCheckToRunCode(resolver, () => runCode(resolver.Resolve<Forth>())));
			}
			CheckApprovalTestResult();
		}

		protected void Start<First, Second, Third, Forth>(Type resolverType,
			Action<First, Second> initCode, Action<Third, Forth> runCode)
		{
			if (IgnoreSlowTestIfStartedViaNCrunchOrNunitConsole(resolverType))
				return; //ncrunch: no coverage

			using (var resolver = CreateResolver(resolverType))
			{
				resolver.RegisterSingleton<Third>();
				resolver.RegisterSingleton<Forth>();
				resolver.Start(initCode,
					AddApprovalCheckToRunCode(resolver,
						() => runCode(resolver.Resolve<Third>(), resolver.Resolve<Forth>())));
			}
			CheckApprovalTestResult();
		}

		protected void Start<First, Second, Third, Forth>(Type resolverType, Action<First> initCode,
			Action<Second, Third, Forth> runCode)
		{
			if (IgnoreSlowTestIfStartedViaNCrunchOrNunitConsole(resolverType))
				return; //ncrunch: no coverage

			using (var resolver = CreateResolver(resolverType))
			{
				resolver.RegisterSingleton<Second>();
				resolver.RegisterSingleton<Third>();
				resolver.RegisterSingleton<Forth>();
				resolver.Start(initCode,
					AddApprovalCheckToRunCode(resolver,
						() =>
							runCode(resolver.Resolve<Second>(), resolver.Resolve<Third>(), resolver.Resolve<Forth>())));
			}
			CheckApprovalTestResult();
		}

		protected void Start<First, Second, Third, Forth, Fifth>(Type resolverType,
			Action<First, Second, Third> initCode, Action<Forth, Fifth> runCode)
		{
			if (IgnoreSlowTestIfStartedViaNCrunchOrNunitConsole(resolverType))
				return; //ncrunch: no coverage

			using (var resolver = CreateResolver(resolverType))
			{
				resolver.RegisterSingleton<Forth>();
				resolver.RegisterSingleton<Fifth>();
				resolver.Start(initCode,
					AddApprovalCheckToRunCode(resolver,
						() => runCode(resolver.Resolve<Forth>(), resolver.Resolve<Fifth>())));
			}
			CheckApprovalTestResult();
		}

		protected void Start<First, Second, Third, Forth, Fifth>(Type resolverType,
			Action<First, Second> initCode, Action<Third, Forth, Fifth> runCode)
		{
			if (IgnoreSlowTestIfStartedViaNCrunchOrNunitConsole(resolverType))
				return; //ncrunch: no coverage

			using (var resolver = CreateResolver(resolverType))
			{
				resolver.RegisterSingleton<Third>();
				resolver.RegisterSingleton<Forth>();
				resolver.RegisterSingleton<Fifth>();
				resolver.Start(initCode,
					AddApprovalCheckToRunCode(resolver,
						() =>
							runCode(resolver.Resolve<Third>(), resolver.Resolve<Forth>(), resolver.Resolve<Fifth>())));
			}
			CheckApprovalTestResult();
		}
	}
}