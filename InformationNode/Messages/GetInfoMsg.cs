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
			lock (CurrentClient.InitNode.LinkedNodes)
			{
				CountdownEvent cde = new CountdownEvent(CurrentClient.InitNode.LinkedNodes.Count);
				foreach (var ln in CurrentClient.InitNode.LinkedNodes)
				{
					try
					{
						Task.Run(() =>
						{
							TcpClient client = new TcpClient(ln.Address, ln.Port);
							var stream = client.GetStream();
							byte[] data = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(this));
							// отправка сообщения
							stream.Write(data, 0, data.Length);
							Thread thread = new Thread(() =>
							{
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
										lock (nodeData)
										{
											nodeData.AddRange(JsonConvert.DeserializeObject<List<Person>>(builder.ToString()));
										}
										break;
									}
								}
							});
							thread.Start();
							Thread.Sleep(30000);
							thread.Abort();
							cde.Signal();
						});

					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
						cde.Signal();
					}
					cde.Wait();
				}
			}
			return new ResponseMsg(JsonConvert.SerializeObject(CurrentClient.InitNode), 
				JsonConvert.SerializeObject(nodeData)).GetResponse();
		}

		public GetInfoMsg(Message msg, Client client) : base(msg, client)
		{
		}
	}
}
