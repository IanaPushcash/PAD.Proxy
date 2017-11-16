using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InformationNode.Messages;
using Newtonsoft.Json;

namespace InformationNode
{
	class Node
	{
		private TcpListener listener; 
		public string FilePath { get; set; }
		public int Port { get; set; }
		//public int PortUdp { get; set; }
		//public int CountLinks { get; set; }
		public string Address { get; set; } = "127.0.0.1";
		public List<LinkedNode> LinkedNodes { get; set; } //ко мне подключаются
		public List<LinkedNode> MyNodes { get; set; } //я подключаюсь к
		private Socket udpSocket { get; set; }
		public Node() { }
		public Node(string filePath, int port, string ip)
		{
			FilePath = filePath;
			Port = port;
			Address = ip;
			//PortUdp = Port + 4000;
			LinkedNodes = new List<LinkedNode>();
			MyNodes = new List<LinkedNode>();
		}

		public void Start()
		{
			try
			{
				StartUdpMulticastListener();
				listener = new TcpListener(IPAddress.Parse(Address), Port);
				listener.Start();
				//Console.WriteLine("Ожидание подключений...");

				while (true)
				{
					TcpClient client = listener.AcceptTcpClient();
					Client clientObj = new Client(client,this);

					// создаем новый поток для обслуживания нового клиента
					Thread clientThread = new Thread(new ThreadStart(clientObj.Process));
					clientThread.Start();
					//Thread.Sleep(100);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			finally
			{
				listener?.Stop();
				udpSocket?.Close();
			}
		}

		private void StartUdpMulticastListener()
		{
			//почистить код
			Task.Run(() =>
			{
				try
				{
					var nmo = new NodeMulticastOption();
					nmo.mcastAddress = IPAddress.Parse("224.168.100.2");
					nmo.mcastPort = 12000;

					// Start a multicast group.
					nmo.StartMulticast();
					nmo.MulticastOptionProperties();
					// Receive broadcast messages.
					nmo.ReceiveBroadcastMessages(this);



					//udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
					//IPAddress ip = IPAddress.Parse("224.5.6.7");
					//IPEndPoint ipep = new IPEndPoint(IPAddress.Any, PortUdp);
					//udpSocket.Bind(ipep);
					//udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
					//	new MulticastOption(ip, IPAddress.Any));
					//while (true)
					//{
					//	byte[] b = new byte[100000];
					//	udpSocket.Receive(b);
					//	string str = Encoding.Unicode.GetString(b, 0, b.Length);
					//	Message medMsg = JsonConvert.DeserializeObject<Message>(str);
					//	Message msg = Message.Create(str, new Client(null, this));
					//	b = Encoding.Unicode.GetBytes(msg.GetResponse());
					//	SendUdpUnicast(int.Parse(medMsg.Body), medMsg.Author);
					//}
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
				finally
				{
					udpSocket?.Close();
				}
				
			});
		}

		public List<Person> GetData()
		{
			if (File.Exists(FilePath))
			{
				var str = File.ReadAllText(FilePath);
				return JsonConvert.DeserializeObject<List<Person>>(str);
			}
			return new List<Person>();
		}

		
		public void SendConnect(Node node)
		{
			try
			{
				TcpClient client = new TcpClient(node.Address, node.Port);
				var stream = client.GetStream();
				Message msg = new Message()
				{
					Body = JsonConvert.SerializeObject(this),
					Type = "Connect",
					Author = Port + ""
				};
				byte[] data = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(msg));
				// отправка сообщения
				stream.Write(data, 0, data.Length);
				stream?.Close();
				client?.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}
