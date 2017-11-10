using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace InformationNode
{
	class LinkedNode:Node
	{
		public LinkedNode(string filePath, int port, string ip) : base(filePath, port, ip)
		{
		}

		public LinkedNode(int port, string ip):base()
		{
			Port = port;
			Address = ip;
		}

		
	}
}
