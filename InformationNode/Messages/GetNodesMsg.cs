using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InformationNode.Messages
{
	class GetNodesMsg:Message
	{
		

		public override string GetResponse()
		{
			CountdownEvent cde = new CountdownEvent(CurrentClient.InitNode.LinkedNodes.Count);
			for ( int i = 0; i < CurrentClient.InitNode.LinkedNodes.Count; i++)
			{
				var ln = CurrentClient.InitNode.LinkedNodes[i];
				try
				{
					Task.Run(() =>
					{
						TcpClient client = new TcpClient(ln.Address, ln.Port);
						if (!client.Connected)
						{
							CurrentClient.InitNode.LinkedNodes.Remove(ln);
							i--;
						}
						cde.Signal();
					});
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					cde.Signal();
				}
			}
			cde.Wait();
			return new ResponseMsg(JsonConvert.SerializeObject(CurrentClient.InitNode), "").GetResponse();
		}

		public GetNodesMsg(Message msg, Client client) : base(msg, client)
		{
		}
	}
}
