using System;

namespace Mediator.Messages
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
