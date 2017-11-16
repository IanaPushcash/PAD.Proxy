using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

// This sender example must be used in conjunction with the listener program.
// You must run this program as follows:
// Open a console window and run the listener from the command line. 
// In another console window run the sender. In both cases you must specify 
// the local IPAddress to use. To obtain this address,  run the ipconfig command 
// from the command line. 
//  
namespace Mediator
{
	class UdpHandler
	{

		public IPAddress mcastAddress;
		public int mcastPort;
		public Socket mcastSocket;
		public void JoinMulticastGroup()
		{
			try
			{
				// Create a multicast socket.
				mcastSocket = new Socket(AddressFamily.InterNetwork,
					SocketType.Dgram,
					ProtocolType.Udp);

				// Get the local IP address used by the listener and the sender to
				// exchange multicast messages. 
				IPAddress localIPAddr = IPAddress.Parse("127.0.0.1");

				// Create an IPEndPoint object. 
				IPEndPoint IPlocal = new IPEndPoint(localIPAddr, 0);

				// Bind this endpoint to the multicast socket.
				mcastSocket.Bind(IPlocal);

				// Define a MulticastOption object specifying the multicast group 
				// address and the local IP address.
				// The multicast group address is the same as the address used by the listener.
				MulticastOption mcastOption;
				mcastOption = new MulticastOption(mcastAddress, localIPAddr);

				mcastSocket.SetSocketOption(SocketOptionLevel.IP,
					SocketOptionName.AddMembership,
					mcastOption);

			}
			catch (Exception e)
			{
				Console.WriteLine("\n" + e);
			}
		}

		public void BroadcastMessage(string message)
		{
			IPEndPoint endPoint;

			try
			{
				//Send multicast packets to the listener.
				endPoint = new IPEndPoint(mcastAddress, mcastPort);
				mcastSocket.SendTo(Encoding.Unicode.GetBytes(message), endPoint);
				Console.WriteLine("Multicast data sent.....");
			}
			catch (Exception e)
			{
				Console.WriteLine("\n" + e);
			}

			mcastSocket?.Close();
		}

		
	}
}