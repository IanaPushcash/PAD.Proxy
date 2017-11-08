using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Mediator.Messages;
using Newtonsoft.Json;

namespace Mediator
{
	class MedUdpClient
	{
		public List<Node> Nodes { get; set; }

		public void FindNodes()
		{
			var msg = new GetNodesMsg("GetNodes");
			Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			IPAddress ip = IPAddress.Parse("224.5.6.7");
			s.SetSocketOption(SocketOptionLevel.IP,SocketOptionName.AddMembership, new MulticastOption(ip));
			s.SetSocketOption(SocketOptionLevel.IP,SocketOptionName.MulticastTimeToLive, 2);
			IPEndPoint ipep = new IPEndPoint(ip, 4567);
			s.Connect(ipep);


			var data = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(msg));
			s.Send(data, data.Length, SocketFlags.None);
			s.Close();


		}
	}
}
