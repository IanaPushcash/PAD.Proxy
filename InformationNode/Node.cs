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
		//public int CountLinks { get; set; }
		public string Address { get; set; } = "127.0.0.1";
		public List<LinkedNode> LinkedNodes { get; set; }
		private Socket udpSocket { get; set; }
		public Node() { }
		public Node(string filePath, int port)
		{
			FilePath = filePath;
			Port = port;
			LinkedNodes = new List<LinkedNode>();
		}

		public void Start()
		{
			try
			{
				StartUdpListener();
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
				Console.WriteLine(ex.Message);
			}
			finally
			{
				listener?.Stop();
				udpSocket?.Close();
			}
		}

		private void StartUdpListener()
		{
			//почистить код
			Task.Run(() =>
			{
				udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,ProtocolType.Udp);
				IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 4567);
				udpSocket.Bind(ipep);
				IPAddress ip = IPAddress.Parse("224.5.6.7");
				udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));
				while (true)
				{
					byte[] b = new byte[100000];
					udpSocket.Receive(b);
					string str = Encoding.Unicode.GetString(b, 0, b.Length);
					TcpClient client = new TcpClient(Address, 11000);
					Message msg = Message.Create(str, new Client(client, this));
					b = Encoding.Unicode.GetBytes(msg.GetResponse());
					client.GetStream().Write(b, 0, b.Length);
					client.Close();
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
	}
}
