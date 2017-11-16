using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InformationNode.Messages
{
	class GetInfoMsg:Message
	{

		public override string GetResponse()
		{
			var nodeData = CurrentClient.InitNode.GetData();
			Console.WriteLine($"GetInfoMsg is recieved");
			if (Author == "Mediator")
			{
				var allNodes = new List<LinkedNode>(CurrentClient.InitNode.LinkedNodes);
				Console.WriteLine($"My own data is added");
				allNodes.AddRange(CurrentClient.InitNode.MyNodes);
				CountdownEvent cde = new CountdownEvent(allNodes.Count);
				foreach (var ln in allNodes)
				{
					try
					{
						Task.Run(() =>
						{
							TcpClient client = new TcpClient(ln.Address, ln.Port);
							var stream = client.GetStream();
							var newMsg = new Message(this)
							{
								Author = CurrentClient.InitNode.Port+""
							};
							byte[] data = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(newMsg));
							// отправка сообщения
							stream.Write(data, 0, data.Length);
							//Thread thread = new Thread(() =>
							//{
							while (true)
							{
								if (stream.DataAvailable)
								{
									// получаем ответ
									data = new byte[1000000]; // буфер для получаемых данных
									StringBuilder builder = new StringBuilder();
									int bytes = 0;
									do
									{
										bytes = stream.Read(data, 0, data.Length);
										builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
									} while (stream.DataAvailable);
									Console.WriteLine($"Node's {ln.Port} data is added");
									nodeData.AddRange(
										JsonConvert.DeserializeObject<List<Person>>(JsonConvert.DeserializeObject<Message>(builder.ToString()).Body));
									//stream?.Close();
									//client?.Close();
									break;
								}
							}
							//});
							//thread.Start();
							//Thread.Sleep(5000);
							//thread.Abort();
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
			}
			Console.WriteLine($"{JsonConvert.SerializeObject(nodeData)}");
			return new ResponseMsg(JsonConvert.SerializeObject(CurrentClient.InitNode),
				JsonConvert.SerializeObject(nodeData)).GetResponse();
		}

		public GetInfoMsg(Message msg, Client client) : base(msg, client)
		{
		}
	}
}
