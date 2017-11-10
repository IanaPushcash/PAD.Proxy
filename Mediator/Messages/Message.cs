using Newtonsoft.Json;

namespace Mediator.Messages
{
	abstract class Message
	{
		public string Author { get; set; }
		public string Type { get; set; }
		public string Body { get; set; }
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

		public abstract string GetResponse();

		public Message(Message msg, Client client)
		{
			Author = msg.Author;
			Type = msg.Type;
			Body = msg.Body;
			CurrentClient = client;
		}
		public Message() { }
	}
}
