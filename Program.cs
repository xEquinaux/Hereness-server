using NetworkMan;
using System;
using System.Net.Sockets;
using System.Text;

namespace Hereness_server
{
	internal class Program
	{
		static void Main(string[] args)
		{
			new PacketManager();
			new Entry("register");
			Entry.Load();
			PacketManager.Instance.RegisterPacketHandler((int)PacketId.Message, SendMessage);
			(ChatServer.Instance = new ChatServer()).Start(8000);
		}
		public static void SendMessage(Packet packet, UdpClient u)
		{
			byte[] buffer = packet.MessageIntoBytes();
			foreach (var item in ChatServer.Instance._client)
			{
				u.Send(buffer, buffer.Length, item.remoteEndpoint);
			}
		}
	}
	public class ChatServer : Server
	{
		public static ChatServer? Instance;
		public override void HandleMessage(Packet packet, Entry e, UdpClient u)
		{
			try
			{ 
				PacketManager.Instance.HandleIncomingPacket(packet, u);
			}
			catch { }
			string status = "";
			byte[] buffer = default;
			switch (packet.Id)
			{
				case (int)PacketId.Status:
					buffer = Encoding.UTF8.GetBytes("SUCCESS");
					u.Send(buffer, buffer.Length, e.remoteEndpoint);
					break;
				case (int)PacketId.Login:
					var endPoint = _client.FirstOrDefault(t => t.remoteEndpoint == e.remoteEndpoint);
					if (endPoint == default)
					{
						int index = _client.IndexOf(endPoint);
						string data = Encoding.UTF8.GetString(packet.Data);
						e.AddEntry(data.Split(' ')[0], "login", data.Split(' ')[1]);
					}
					buffer = Encoding.UTF8.GetBytes("SUCCESS");
					u.Send(buffer, buffer.Length, e.remoteEndpoint);
					break;
				case (int)PacketId.Message:
					//buffer = packet.MessageIntoBytes();
					//foreach (var item in Instance._client)
					//{
					//	u.Send(buffer, buffer.Length, item.remoteEndpoint);
					//}
					break;
			}
		}
	}
}
