using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace InformationNode.Messages
{
	class GetNodesMsg:Message
	{
		

		public override string GetResponse()
		{
			CurrentClient.InitNode.LinkedNodes = GetValue(CurrentClient.InitNode.LinkedNodes);
			CurrentClient.InitNode.MyNodes = GetValue(CurrentClient.InitNode.MyNodes);
			Console.WriteLine($"Return nodes");
			return new ResponseMsg(CurrentClient.InitNode.Port+"", JsonConvert.SerializeObject(CurrentClient.InitNode).Replace("\"", "~"), ReturnJson).GetResponse();
		}

		private List<Node> GetValue(List<Node> nodes)
		{
			for (int i = 0; i < nodes.Count; i++)
			{
				var ln = nodes[i];
				try
				{

					TcpClient client = new TcpClient(ln.Address, ln.Port);
					if (!client.Connected)
					{
						nodes.Remove(ln);
						i--;
					}
					client.Close();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
			}
			return nodes;
		}

		public GetNodesMsg(Message msg, Client client) : base(msg, client)
		{
		}
	}
}
