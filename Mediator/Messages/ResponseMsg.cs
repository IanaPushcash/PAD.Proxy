using Newtonsoft.Json;

namespace Mediator.Messages
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
