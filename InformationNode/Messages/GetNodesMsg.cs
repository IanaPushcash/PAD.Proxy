using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationNode.Messages
{
	class GetNodesMsg:Message
	{
		

		public override string GetResponse()
		{
			throw new NotImplementedException();
		}

		public GetNodesMsg(Message msg) : base(msg)
		{
		}
	}
}
