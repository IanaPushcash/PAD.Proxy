using System;

namespace Mediator.Messages
{
	class GetNodesMsg:Message
	{
		

		public override string GetResponse()
		{
			throw new NotImplementedException();
		}

		public GetNodesMsg(Message msg, Client client) : base(msg, client)
		{
		}

		public GetNodesMsg() { }
	}
}
