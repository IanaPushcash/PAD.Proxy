using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationNode.Messages
{
	class ErrorMsg:Message
	{
		public override string GetResponse()
		{
			throw new NotImplementedException();
		}

		public ErrorMsg(Message msg) : base(msg)
		{
		}

		public ErrorMsg():base()
		{
			
		}
	}
}
