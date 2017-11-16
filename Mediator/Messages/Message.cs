using Newtonsoft.Json;

namespace Mediator.Messages
{
	class Message
	{
		public string Author { get; set; }
		public string Type { get; set; }
		public string Body { get; set; }
		public Filter FilterBy { get; set; }
		public Sort SortBy { get; set; }
		public bool ReturnJson { get; set; }
		protected Client CurrentClient { get; set; }
		public static Message Create(string msg, Client client)
		{
			var msgObj = JsonConvert.DeserializeObject<Message>(msg);
			switch (msgObj.Type)
			{
				case "GetInfo": return new GetInfoMsg(msgObj, client);
				case "GetNodes":return new GetNodesMsg(msgObj, client);
				default:
					return new ErrorMsg();
			}
			
		}

		public virtual string GetResponse()
		{
			return "";
		}

		public Message(Message msg, Client client)
		{
			Author = msg.Author;
			Type = msg.Type;
			Body = msg.Body;
			CurrentClient = client;
			FilterBy = msg.FilterBy;
			SortBy = msg.SortBy;
			ReturnJson = msg.ReturnJson;
		}
		public Message() { }

		public Message(Message msg)
		{
			Author = msg.Author;
			Type = msg.Type;
			Body = msg.Body;
			FilterBy = msg.FilterBy;
			SortBy = msg.SortBy;
			ReturnJson = msg.ReturnJson;
		}
	}
}
