using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InformationNode.Messages;
using Newtonsoft.Json;

namespace InformationNode
{
	class Client
	{
		public TcpClient client;
		public string ClientName { get; set; }
		public Node InitNode { get; set; }
		public Client(TcpClient tcpClient, Node node)
		{
			client = tcpClient;
			InitNode = node;
		}
		
		
		public void Process()
		{
			NetworkStream stream = null;
			try
			{
				stream = client.GetStream();

				byte[] data = new byte[1000000]; // буфер для получаемых данных
				// получаем сообщение
				StringBuilder builder = new StringBuilder();
				int bytes = 0;
				//while (true)
				//{
				//	if (stream.DataAvailable)
				//	{
				do
				{
					bytes = stream.Read(data, 0, data.Length);
					builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
				} while (stream.DataAvailable);
				if (builder.Length > 0)
				{
					Message msg = Message.Create(builder.ToString(), this);
					string response = msg.GetResponse();
					data = Encoding.Unicode.GetBytes(response);
					stream.Write(data, 0, data.Length);
				}
				//		break;
				//	}
				//}
				//ClientName = msg.Name;
				//if (!msg.IsSender && !Broker.Subscribers.Contains(this))
				//{
				//	TargetAuthor = msg.Name;
				//	TargetType = msg.TypeMsg;
				//	Broker.Subscribers.Add(this);
				//}
				//else if (!Broker.Publishers.Contains(this))
				//{
				//	Broker.Publishers.Add(this);
				//}
				//Broker.GetInstance().ProcessingMsg(msg, stream);

				//	while (true)
				//	{
				//		stream.
				//	}
				//	Broker.GetInstance().AddMsg(msg);
				//else Broker.GetInstance().GetAnswerMsg(msg, stream);
				// отправляем обратно сообщение в верхнем регистре
				//message = message.Substring(message.IndexOf(':') + 1).Trim().ToUpper();

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			finally
			{
				//if (stream != null)
				//	stream.Close();
				//if (client != null)
				//	client.Close();
				stream?.Close();
				client?.Close();
			}
		}
		

	} 
}
