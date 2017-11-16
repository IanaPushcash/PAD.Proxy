using System;
using Newtonsoft.Json;

namespace InformationNode.Messages
{
	class ConnectMsg:Message
	{
		
		public override string GetResponse()
		{
			Node lNode = JsonConvert.DeserializeObject<Node>(Body);
			CurrentClient.InitNode.LinkedNodes.Add(lNode);
			Console.WriteLine($"node {lNode.Port} is connected");
			return new ResponseMsg(JsonConvert.SerializeObject(CurrentClient.InitNode), "success", ReturnJson).GetResponse();
		}

		public ConnectMsg(Message msg, Client client) : base(msg, client)
		{
		}
	}
}
