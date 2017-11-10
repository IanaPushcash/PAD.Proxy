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
		public List<LinkedNode> LinkedNodes { get; set; } //ко мне подключаются
		public List<LinkedNode> MyNodes { get; set; } //я подключаюсь к
		private Socket udpSocket { get; set; }
		public Node() { }
		public Node(string filePath, int port, string ip)
		{
			FilePath = filePath;
			Port = port;
			Address = ip;
			LinkedNodes = new List<LinkedNode>();
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
				Console.WriteLine(ex.Message);
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
					udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
					IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 4567);
					udpSocket.Bind(ipep);
					IPAddress ip = IPAddress.Parse("224.5.6.7");
					udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
						new MulticastOption(ip, IPAddress.Any));
					while (true)
					{
						byte[] b = new byte[100000];
						udpSocket.Receive(b);
						string str = Encoding.Unicode.GetString(b, 0, b.Length);
						Message medMsg = JsonConvert.DeserializeObject<Message>(str);
						Message msg = Message.Create(str, new Client(null, this));
						b = Encoding.Unicode.GetBytes(msg.GetResponse());
						SendUdpUnicast(int.Parse(medMsg.Body), medMsg.Author);
					}
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

		public void SendUdpUnicast(int port, string ip)
		{
			IPEndPoint remoteep = new IPEndPoint(IPAddress.Parse(ip), port);
			UdpClient client = new UdpClient(remoteep);
			Message msg = new ResponseMsg(Port+"", JsonConvert.SerializeObject(this));

			var b = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(msg));
			client.Send(b, b.Length, remoteep);
			client.Close();
		}

		public void SendConnect(Node node)
		{
			try
			{
				TcpClient client = new TcpClient(node.Address, node.Port);
				var stream = client.GetStream();
				byte[] data = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(this));
				// отправка сообщения
				stream.Write(data, 0, data.Length);
				stream?.Close();
				client?.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}
}
