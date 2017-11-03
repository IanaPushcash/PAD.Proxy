using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationNode.Messages
{
	class GetInfoMsg:Message
	{
		
		public override string GetResponse()
		{
			throw new NotImplementedException();
		}

		public GetInfoMsg(Message msg) : base(msg)
		{
		}
	}
}
