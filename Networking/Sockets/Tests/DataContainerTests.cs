using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DeltaEngine.Networking.Sockets.Tests
{
	public class DataContainerTests
	{
		[SetUp]
		public void InitializeObjectsList()
		{
			collectedDataObjects = new List<MessageData>();
		}

		[Test]
		public void CollectData()
		{
			var dataCollector = new DataCollector();
			dataCollector.ObjectFinished += container => collectedDataObjects.Add(container);
			Assert.IsEmpty(collectedDataObjects);
			var bytePackages = SplitDataStream(CreateByteList(), 10, 10, 11, 1);
			foreach (byte[] package in bytePackages)
				dataCollector.ReadBytes(package, 0, package.Length);

			Assert.AreEqual(3, collectedDataObjects.Count);
		}

		private static List<byte> CreateByteList()
		{
			var byteList = new List<byte>();
			byteList.AddRange(DataCollector.GetTestBytesWithLengthHeader(6));
			byteList.AddRange(DataCollector.GetTestBytesWithLengthHeader(4));
			byteList.AddRange(DataCollector.GetTestBytesWithLengthHeader(10));
			Assert.AreEqual(32, byteList.Count);
			return byteList;
		}

		private IEnumerable<byte[]> SplitDataStream(List<byte> dataStream, params int[] splits)
		{
			byte[] allByteData = dataStream.ToArray();
			var bytePackages = new List<byte[]>();
			currentIndex = 0;
			foreach (var byteCount in splits)
				ExtractBytes(byteCount, allByteData, bytePackages);

			ExtractRemainingBytes(allByteData, bytePackages);
			return bytePackages;
		}

		private int currentIndex;

		private void ExtractBytes(int byteCount, byte[] allByteData, List<byte[]> bytePackages)
		{
			var package = new byte[byteCount];
			Array.Copy(allByteData, currentIndex, package, 0, package.Length);
			bytePackages.Add(package);
			currentIndex += byteCount;
		}

		private void ExtractRemainingBytes(byte[] allByteData, List<byte[]> bytePackages)
		{
			int remainingBytes = allByteData.Length - currentIndex;
			if (remainingBytes <= 0)
				return;

			var lastPackage = new byte[remainingBytes];
			Array.Copy(allByteData, currentIndex, lastPackage, 0, lastPackage.Length);
			bytePackages.Add(lastPackage);
		}

		private List<MessageData> collectedDataObjects;

		[TearDown]
		public void DisposeObjectsList()
		{
			collectedDataObjects = null;
		}
	}
}