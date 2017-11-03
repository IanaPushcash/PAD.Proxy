using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InformationNode.Messages
{
	class ResponseMsg:Message
	{
		public override string GetResponse()
		{
			return JsonConvert.SerializeObject(this);
		}

		public ResponseMsg(string author, string body)
		{
			Author = author;
			Body = body;
		}
	}
}
