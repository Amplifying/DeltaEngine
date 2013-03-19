using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DeltaEngine.Networking.Sockets.Tests
{
	internal class DataContainerTests
	{
		[Test]
		public void CollectData()
		{
			var dataCollector = new DataCollector();
			dataCollector.ObjectFinished += container => collectedDataObjects.Add(container);
			Assert.IsEmpty(collectedDataObjects);

			var byteList = new List<byte>();
			byteList.AddRange(dataCollector.GetTestBytesWithLengthHeader(6));
			byteList.AddRange(dataCollector.GetTestBytesWithLengthHeader(4));
			byteList.AddRange(dataCollector.GetTestBytesWithLengthHeader(10));
			Assert.AreEqual(32, byteList.Count);

			var bytePackages = SplitDataStream(byteList, 10, 10, 11, 1);
			foreach (byte[] package in bytePackages)
				dataCollector.ReadBytes(package, 0, package.Length);

			Assert.AreEqual(3, collectedDataObjects.Count);
		}

		private List<byte[]> SplitDataStream(List<byte> dataStream, params int[] splits)
		{
			byte[] allByteData = dataStream.ToArray();
			int currentIndex = 0;

			var bytePackages = new List<byte[]>();
			foreach (var byteCount in splits)
			{
				var package = new byte[byteCount];
				Array.Copy(allByteData, currentIndex, package, 0, package.Length);
				bytePackages.Add(package);
				currentIndex += byteCount;
			}

			int remainingBytes = allByteData.Length - currentIndex;
			if (remainingBytes > 0)
			{
				var lastPackage = new byte[remainingBytes];
				Array.Copy(allByteData, currentIndex, lastPackage, 0, lastPackage.Length);
				bytePackages.Add(lastPackage);
			}

			return bytePackages;
		}

		[SetUp]
		public void InitializeObjectsList()
		{
			collectedDataObjects = new List<MessageData>();
		}

		private List<MessageData> collectedDataObjects;

		[TearDown]
		public void DisposeObjectsList()
		{
			collectedDataObjects = null;
		}
	}
}