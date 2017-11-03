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
using Newtonsoft.Json;

namespace InformationNode
{
	class Node
	{
		private TcpListener listener; 
		public string FilePath { get; set; }
		public int Port { get; set; }
		public int CountLinks { get; set; }
		public string Address { get; set; } = "127.0.0.1";
		public List<LinkedNode> LinkedNodes { get; set; }
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
			}
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
