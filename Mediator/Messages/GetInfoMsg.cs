using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Mediator.Messages
{
	class GetInfoMsg : Message
	{

		public override string GetResponse()
		{
			var nodeData = new List<Person>();
			var udpClient = new MedUdpClient();
			udpClient.FindNodes();
			Mediator.SearchMainNodes();
			CountdownEvent cde = new CountdownEvent(Mediator.Nodes.Count);
			foreach (var node in Mediator.Nodes)
			{
				try
				{
					Task.Run(() =>
					{
						TcpClient client = new TcpClient(node.Address, node.Port);
						var stream = client.GetStream();
						byte[] data = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(new GetInfoMsg()
						{
							Type = "GetInfo",
							Author = "Mediator"
						}));
						// отправка сообщения
						stream.Write(data, 0, data.Length);
						while (true)
						{
							if (stream.DataAvailable)
							{
								// получаем ответ
								var data2 = new byte[10000]; // буфер для получаемых данных
								StringBuilder builder = new StringBuilder();
								int bytes = 0;
								do
								{
									bytes = stream.Read(data2, 0, data2.Length);
									builder.Append(Encoding.Unicode.GetString(data2, 0, bytes));
								} while (stream.DataAvailable);
								nodeData.AddRange(
									JsonConvert.DeserializeObject<List<Person>>(JsonConvert.DeserializeObject<Message>(builder.ToString()).Body));
								break;
							}
						}
						stream?.Close();
						client?.Close();
						cde.Signal();
					});

				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
					cde.Signal();
				}

			}
			cde.Wait();
			return new ResponseMsg("Mediator",
				JsonConvert.SerializeObject(nodeData)).GetResponse();
		}


		public GetInfoMsg(Message msg, Client client) : base(msg, client)
		{
		}

		public GetInfoMsg()
		{
		}
	}
}
