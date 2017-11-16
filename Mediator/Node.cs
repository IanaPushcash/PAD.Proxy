using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Mediator
{
	class Node
	{
		public string FilePath { get; set; }
		public int Port { get; set; }
		public string Address { get; set; } = "127.0.0.1";
		public List<Node> LinkedNodes { get; set; } //ко мне подключаются
		public List<Node> MyNodes { get; set; } //я подключаюсь к
		public void SendGetInfoMsg(Node node)
		{
			try
			{
				TcpClient client = new TcpClient(node.Address, node.Port);
				var stream = client.GetStream();
				byte[] data = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(this));
				// отправка сообщения
				stream.Write(data, 0, data.Length);
				stream?.Close();
				client?.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			var n = (Node) obj;
			return Port == n.Port && Address == n.Address;
		}

		public Node() { }
	}
}
