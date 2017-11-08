using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediator
{
	class Node
	{
		public string FilePath { get; set; }
		public int Port { get; set; }
		//public int CountLinks { get; set; }
		public string Address { get; set; } = "127.0.0.1";
		public List<Node> LinkedNodes { get; set; }
	}
}
