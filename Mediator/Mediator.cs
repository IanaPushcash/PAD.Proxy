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
