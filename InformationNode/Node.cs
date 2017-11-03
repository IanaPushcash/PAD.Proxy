using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InformationNode
{
	class Node
	{
		private TcpListener listener; 
		public string FilePath { get; set; }
		public int Port { get; set; }
		public int CountLinks { get; set; }
		public ConcurrentQueue<LinkedNode> LinkedNodes { get; set; }
		public Node(string filePath, int port)
		{
			FilePath = filePath;
			Port = port;
			LinkedNodes = new ConcurrentQueue<LinkedNode>();
		}

		public void Start()
		{
			try
			{
				listener = new TcpListener(IPAddress.Parse("127.0.0.1"), Port);
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
	}
}
