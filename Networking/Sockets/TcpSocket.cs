using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using DeltaEngine.Entities;

namespace DeltaEngine.Networking.Sockets
{
	public class TcpSocket : Client
	{
		public TcpSocket()
			: this(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) {}

		public TcpSocket(Socket nativeSocket)
		{
			this.nativeSocket = nativeSocket;
			buffer = new byte[256];
			dataCollector = new DataCollector();
			dataCollector.ObjectFinished += OnObjectFinished;
			isDisposed = false;
		}

		protected readonly Socket nativeSocket;
		private readonly byte[] buffer;
		private bool isDisposed;
		private readonly DataCollector dataCollector;

		private void OnObjectFinished(MessageData dataContainer)
		{
			using (var dataStream = new MemoryStream(dataContainer.Data))
			using (var dataReader = new BinaryReader(dataStream))
			{
				object receivedMessage = dataReader.Create();
				if (DataReceived != null)
					DataReceived(receivedMessage);
				else
					throw new NoDataReceivedEventWasAttached(receivedMessage);
			}
		}

		public event Action<object> DataReceived;
		
		private class NoDataReceivedEventWasAttached : Exception
		{
			public NoDataReceivedEventWasAttached(object receivedMessage)
				: base(receivedMessage.ToString()) {}
		}

		public void Connect(string serverAddress, int serverPort)
		{
			connectionTargetAddress = serverAddress + ":" + serverPort;
			Connect(serverAddress.ToEndPoint(serverPort));
		}

		private string connectionTargetAddress;

		public void Connect(EndPoint targetAddress)
		{
			try
			{
				var socketArgs = new SocketAsyncEventArgs { RemoteEndPoint = targetAddress };
				socketArgs.Completed += SocketConnectionComplete;
				nativeSocket.ConnectAsync(socketArgs);
			}
			catch (SocketException)
			{
				Console.WriteLine("An error has occurred when trying to request a connection " +
					"to the server (" + targetAddress + ")");
			}
		}

		private void SocketConnectionComplete(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
		{
			lock (syncObject)
			{
				if (socketAsyncEventArgs.SocketError == SocketError.Success)
				{
					WaitForData();
					if (Connected != null)
						Connected();
					SendAllMessagesInTheQueue();
				}
			}
		}

		public event Action Connected;

		public void Send(object data)
		{
			try
			{
				SendOrEnqueueData(data);
			}
			catch (SocketException)
			{
				Dispose();
			}
		}

		private void SendOrEnqueueData(object data)
		{
			lock (syncObject)
			{
				if (IsConnected)
					SendDataThroughNativeSocket(data);
				else
					messages.Enqueue(data);
			}
		}

		private readonly Queue<object> messages = new Queue<object>();
		private readonly Object syncObject = new Object();

		private void SendDataThroughNativeSocket(object message)
		{
			if (nativeSocket == null || isDisposed)
				throw new SocketException();		
			int numberOfSendBytes = nativeSocket.Send(message.ToByteArrayWithLengthHeader());
			if (numberOfSendBytes == 0)
				throw new SocketException();			
		}

		private void SendAllMessagesInTheQueue()
		{
			while (messages.Count > 0)
				SendDataThroughNativeSocket(messages.Dequeue());
		}

		public void WaitForData()
		{
			if (isDisposed)
				return;

			try
			{
				nativeSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceivingBytes, null);
			}
			catch (SocketException)
			{
				Console.WriteLine("An error has occurred when setting the socket to receive data");
			}
		}

		private void ReceivingBytes(IAsyncResult asyncResult)
		{
			if (isDisposed)
				return;

			try
			{
				TryReceiveBytes(asyncResult);
			}
			catch (SocketException)
			{
				Dispose();
			}
			catch (ObjectDisposedException)
			{
				Dispose();
			}
		}

		private void TryReceiveBytes(IAsyncResult asyncResult)
		{
			int numberOfReceivedBytes = nativeSocket.EndReceive(asyncResult);
			if (numberOfReceivedBytes == 0)
				throw new SocketException();

			dataCollector.ReadBytes(buffer, 0, numberOfReceivedBytes);
			WaitForData();
		}

		public string TargetAddress
		{
			get { return IsConnected ? nativeSocket.RemoteEndPoint.ToString() : connectionTargetAddress; }
		}

		public bool IsConnected
		{
			get { return nativeSocket != null && nativeSocket.Connected; }
		}

		public virtual void Dispose()
		{
			if (isDisposed)
				return;

			isDisposed = true;
			nativeSocket.Close();

			if (Disconnected != null)
				Disconnected();
		}

		public event Action Disconnected;
	}
}