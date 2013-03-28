using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Networking.Sockets
{
	public class TcpSocket : ClientConnection
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

		protected Socket nativeSocket;
		private readonly byte[] buffer;
		private bool isDisposed;
		private readonly DataCollector dataCollector;

		private void OnObjectFinished(MessageData dataContainer)
		{
			using (var dataStream = new MemoryStream(dataContainer.Data))
			using (var dataReader = new BinaryReader(dataStream))
			{
				BinaryData receivedMessage = GetReceivedMessage(dataReader);
				if (DataReceived != null)
					DataReceived(this, receivedMessage);
			}
		}

		private static BinaryData GetReceivedMessage(BinaryReader dataReader)
		{
			BinaryData receivedMessage;
			try
			{
				receivedMessage = dataReader.Create<BinaryData>();
			}
			catch (BinaryDataExtension.UnknownMessageTypeReceived ex)
			{
				receivedMessage = new UnknownBinaryData(ex.Message);
			}
			return receivedMessage;
		}

		public void WaitForData()
		{
			if (isDisposed)
				return;

			try
			{
				nativeSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceivingBytes, null);
			}
			catch (SocketException) { }
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
		}

		private void TryReceiveBytes(IAsyncResult asyncResult)
		{
			int numberOfReceivedBytes = nativeSocket.EndReceive(asyncResult);
			if (numberOfReceivedBytes == 0)
				throw new SocketException();

			dataCollector.ReadBytes(buffer, 0, numberOfReceivedBytes);
			WaitForData();
		}

		public void Send(BinaryData data)
		{
			try
			{
				TrySendData(data);
			}
			catch (SocketException)
			{
				Dispose();
			}
}

		private void TrySendData(BinaryData data)
		{
			int numberOfSendBytes = nativeSocket.Send(data.ToByteArrayWithLengthHeader());
			if (numberOfSendBytes == 0)
				throw new SocketException();
		}

		protected virtual void OnReceived(BinaryData message)
		{
			if (DataReceived != null)
				DataReceived(this, message);
		}

		public event Action<ClientConnection, BinaryData> DataReceived;

		public void ConnectAndWaitForData(EndPoint targetAddress)
		{
			try
			{
				nativeSocket.Connect(targetAddress);
				WaitForData();
			}
			catch (SocketException) {}
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
				Disconnected(this);
		}

		public event Action<ClientConnection> Disconnected;
	}
}