using System;
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
				listener = new TcpListener(IPAddress.Parse("127.0.0.1"), Port);
				listener.Start();
				//Console.WriteLine("Ожидание подключений...");

				while (true)
				{
					TcpClient client = listener.AcceptTcpClient();
					LinkedNode lNode = new LinkedNode(client);

					// создаем новый поток для обслуживания нового клиента
					Thread clientThread = new Thread(new ThreadStart(lNode.Process));
					clientThread.Start();
					Thread.Sleep(100);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				if (listener != null)
					listener.Stop();
			}
		}
	}
}
