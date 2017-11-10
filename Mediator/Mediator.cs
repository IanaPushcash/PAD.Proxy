using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mediator
{
	class Mediator
	{
		private TcpListener listener;

		public int Port { get; set; } = 11000;
		//public int CountLinks { get; set; }
		public string Address { get; set; } = "127.0.0.1";
		public static List<Node> Nodes { get; set; }

		public static void SearchMainNodes()
		{
			if (Nodes.Count > 0)
			{
				List<List<Node>> ln = new List<List<Node>>();
				int indexMax = 0;
				for (int i = 0; i < Nodes.Count; i++)
				{
					var node = Nodes[i];
					ln.Add(node.LinkedNodes.Union(node.MyNodes).Distinct().ToList());
					if (i > 0 && ln.Last().Count > ln[ln.Count - 2].Count)
						indexMax = ln.Count - 1;
				}
				var mainNodes = new List<Node>();
				mainNodes.Add(Nodes[indexMax]);
				for (int i = 0; i < ln.Count; i++)
				{
					for (int j = 0; i < ln[i].Count; j++)
					{
						if (i != indexMax && !ln[indexMax].Contains(ln[i][j]))
						{
							ln[indexMax].Add(ln[i][j]);
							mainNodes.Add(Nodes[i]);
						}
					}
				}
				Nodes = mainNodes;
			}
			
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
					Client clientObj = new Client(client, this);

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
