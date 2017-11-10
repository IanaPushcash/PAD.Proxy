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
			CurrentClient.InitNode.LinkedNodes = GetValue(CurrentClient.InitNode.LinkedNodes);
			CurrentClient.InitNode.MyNodes = GetValue(CurrentClient.InitNode.MyNodes);
			return new ResponseMsg(JsonConvert.SerializeObject(CurrentClient.InitNode), "").GetResponse();
		}

		private List<LinkedNode> GetValue(List<LinkedNode> nodes)
		{
			CountdownEvent cde = new CountdownEvent(nodes.Count);
			for (int i = 0; i < nodes.Count; i++)
			{
				var ln = nodes[i];
				try
				{
					Task.Run(() =>
					{
						TcpClient client = new TcpClient(ln.Address, ln.Port);
						if (!client.Connected)
						{
							nodes.Remove(ln);
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
			return nodes;
		}

		public GetNodesMsg(Message msg, Client client) : base(msg, client)
		{
		}
	}
}
