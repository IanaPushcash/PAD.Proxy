﻿using System.Net.Sockets;
using Newtonsoft.Json;

namespace InformationNode.Messages
{
	class Message
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
				case "Connect": return new ConnectMsg(msgObj, client);
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
		}
		public Message() { }
		public Message(Message msg)
		{
			Author = msg.Author;
			Type = msg.Type;
			Body = msg.Body;
		}

		public Message(string author, string type, string body)
		{
			Author = author;
			Type = type;
			Body = body;
		}
	}
}
