using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Mediator.Messages
{
	class GetInfoMsg:Message
	{

		public override string GetResponse()
		{
			var udpClient = new MedUdpClient();
			udpClient.FindNodes();

			return new ResponseMsg(JsonConvert.SerializeObject(CurrentClient.InitNode),
				JsonConvert.SerializeObject(nodeData)).GetResponse();
		}

		public GetInfoMsg(Message msg, Client client) : base(msg, client)
		{
		}
	}
}
