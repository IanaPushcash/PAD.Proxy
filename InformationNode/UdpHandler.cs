using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using InformationNode.Messages;
using Newtonsoft.Json;


// This is the listener example that shows how to use the MulticastOption class. 
// In particular, it shows how to use the MulticastOption(IPAddress, IPAddress) 
// constructor, which you need to use if you have a host with more than one 
// network card.
// The first parameter specifies the multicast group address, and the second 
// specifies the local address of the network card you want to use for the data
// exchange.
// You must run this program in conjunction with the sender program as 
// follows:
// Open a console window and run the listener from the command line. 
// In another console window run the sender. In both cases you must specify 
// the local IPAddress to use. To obtain this address run the ipconfig comand 
// from the command line. 
//  
namespace InformationNode
{

	class UdpHandler
	{

		public IPAddress mcastAddress;
		public int mcastPort;
		public Socket mcastSocket;
		public MulticastOption mcastOption;


		private void StartMulticast()
		{

			try
			{
				mcastSocket = new Socket(AddressFamily.InterNetwork,
					SocketType.Dgram,
					ProtocolType.Udp);

				IPAddress localIPAddr = IPAddress.Parse("127.0.0.1");

				//IPAddress localIP = IPAddress.Any;
				EndPoint localEP = (EndPoint)new IPEndPoint(IPAddress.Any, 12000);
				mcastOption = new MulticastOption(mcastAddress, localIPAddr);

				mcastSocket.SetSocketOption(SocketOptionLevel.Socket,
					SocketOptionName.ReuseAddress,
					true);
				mcastSocket.Bind(localEP);


				// Define a MulticastOption object specifying the multicast group 
				// address and the local IPAddress.
				// The multicast group address is the same as the address used by the server.

				mcastSocket.SetSocketOption(SocketOptionLevel.IP,
					SocketOptionName.AddMembership,
					mcastOption);

			}

			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		private void ReceiveBroadcastMessages(Node currentNode)
		{
			EndPoint remoteEP = (EndPoint)new IPEndPoint(IPAddress.Any, 0);
			try
			{
				while (true)
				{
					byte[] b = new byte[100000];
					mcastSocket.ReceiveFrom(b, ref remoteEP);
					string str = Encoding.Unicode.GetString(b, 0, b.Length);
					Console.WriteLine($"Получен Udp запрос");
					Message medMsg = JsonConvert.DeserializeObject<Message>(str);
					Message msg = Message.Create(str, new Client(null, currentNode));
					b = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(msg.GetResponse()));
					SendUdpUnicast(int.Parse(medMsg.Body), medMsg.Author, currentNode.Port, b);
				}
			}

			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
			mcastSocket?.Close();
		}

		private void SendUdpUnicast(int port, string ip, int currentPort, byte[] msg)
		{
			IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(ip), 0);
			IPEndPoint targetEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
			UdpClient client = new UdpClient(localEndPoint);
			client.Send(msg, msg.Length, targetEndPoint);
			client.Close();
			Console.WriteLine($"Отправлен unicast");
		}

		private void MulticastOptionProperties()
		{
			Console.WriteLine("Current multicast group is: " + mcastOption.Group);
			Console.WriteLine("Current multicast local address is: " + mcastOption.LocalAddress);
		}

		public void StartUdpMulticastListenerAsync(Node node)
		{
			Task.Run(() =>
			{
				try
				{
					var nmo = new UdpHandler();
					nmo.mcastAddress = IPAddress.Parse("224.168.100.2");
					nmo.mcastPort = 12000;

					// Start a multicast group.
					nmo.StartMulticast();
					nmo.MulticastOptionProperties();
					// Receive broadcast messages.
					nmo.ReceiveBroadcastMessages(node);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}

			});
		}
	}
}
