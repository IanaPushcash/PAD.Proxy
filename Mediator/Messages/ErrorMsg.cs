using System;
using Newtonsoft.Json;

namespace Mediator.Messages
{
	class ErrorMsg:Message
	{
		public override string GetResponse()
		{
			return JsonConvert.SerializeObject(this);
		}

		public ErrorMsg(Message msg) : base()
		{
		}

		public ErrorMsg():base()
		{
			
		}
	}
}
