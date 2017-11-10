using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
			var msg = new GetNodesMsg()
			{
				Author = "127.0.0.1",
				Body = "10000",
				Type = "GetNodes"
			};
			Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			IPAddress ip = IPAddress.Parse("224.5.6.7");
			s.SetSocketOption(SocketOptionLevel.IP,SocketOptionName.AddMembership, new MulticastOption(ip));
			s.SetSocketOption(SocketOptionLevel.IP,SocketOptionName.MulticastTimeToLive, 2);
			IPEndPoint ipep = new IPEndPoint(ip, 4567);
			s.Connect(ipep);


			var data = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(msg));
			s.Send(data, data.Length, SocketFlags.None);
			s.Close();


			UdpClient udpclient = new UdpClient();
			IPEndPoint localEp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10000);
			udpclient.Client.Bind(localEp);
			udpclient.Client.ReceiveTimeout = 10000;
			bool isTimeExpired = false;
			while (!isTimeExpired)
			{
				try
				{
					byte[] b = udpclient.Receive(ref localEp);
					string str = Encoding.Unicode.GetString(b, 0, b.Length);
					Message nodeMsg = JsonConvert.DeserializeObject<Message>(str);
					Mediator.Nodes.Add(JsonConvert.DeserializeObject<Node>(nodeMsg.Body));
				}
				catch (SocketException)
				{
					isTimeExpired = true;
					continue;
				}
				Thread.Sleep(100);
			}
			udpclient.Close();

		}
	}
}
