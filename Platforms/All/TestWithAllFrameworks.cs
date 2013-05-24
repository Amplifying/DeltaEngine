using System;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Platforms.All
{
	/// <summary>
	/// Extends test classes to be able to run it with any resolver and graphic framework.
	/// http://DeltaEngine.net/About/CodingStyle#Tests
	/// </summary>
	public abstract class TestWithAllFrameworks : TestWithMockResolver
	{
		//ncrunch: no coverage start
		//// ReSharper disable ConvertToConstant.Global
		//// ReSharper disable FieldCanBeMadeReadOnly.Global
		public static readonly Type OpenGL = typeof(OpenTKResolver);
		public static readonly Type DirectX = typeof(SharpDxResolver);
		public static readonly Type DirectX9 = typeof(SlimDxResolver);
		public static readonly Type Xna = typeof(XnaResolver);
		/// <summary>
		/// NCrunch will always execute all resolvers as it does not understand the Ignore, but only
		/// TestResolver will be executed (rest is ignored by default). ReSharper will ignore all test
		/// cases with Ignore (e.g. with F6), but you can still execute them manually if you like.
		/// </summary>
		public static readonly TestCaseData[] AllResolvers =
		{
			new TestCaseData(typeof(MockResolver)),
			new TestCaseData(OpenGL),
			new TestCaseData(DirectX).Ignore(),
			new TestCaseData(DirectX9).Ignore(),
			new TestCaseData(Xna).Ignore()
		};

		/// <summary>
		/// By default all slow integration tests (using a non test resolver) are excluded from NCrunch
		/// runs. You can either temporary enable them all here or just selectivly in derived classes.
		/// </summary>
		protected override bool NCrunchAllowIntegrationTests
		{
			get { return false; }
		}

		/// <summary>
		/// Use this flag and comment out all .Ignore calls in the Resolvers list to test all Visual
		/// and Integration tests with one frame. Used to test approval tests with all resolvers to
		/// make sure everything is fully working (DX, XNA, GL). Warning, each test takes a few seconds.
		/// </summary>
		protected override bool ForceAllVisualTestsToBehaveLikeIntegrationTests
		{
			get { return false; }
		}
	}
}