using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
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
