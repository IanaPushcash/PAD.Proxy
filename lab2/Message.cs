using System.Net.Sockets;
using Newtonsoft.Json;

namespace InformationNode.Messages
{
	public class Message
	{
		public string Author { get; set; }
		public string Type { get; set; }
		public string Body { get; set; }
		
	}
}
