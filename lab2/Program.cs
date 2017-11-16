using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InformationNode.Messages;
using Newtonsoft.Json;

namespace lab2
{
	class Program
	{
		static void Main(string[] args)
		{ 
			var x = new Message()
			{
				Author = "I",
				Body = "All",
				Type = "GetInfo"
			};
			Console.WriteLine(JsonConvert.SerializeObject(x));
			TcpClient client = new TcpClient("127.0.0.1", 11000);
			var stream = client.GetStream();
			byte[] data = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(x));
			// отправка сообщения
			stream.Write(data, 0, data.Length);
			Thread thread = new Thread(() =>
			{
				while (true)
				{
					if (stream.DataAvailable)
					{
						// получаем ответ
						data = new byte[1000000]; // буфер для получаемых данных
						StringBuilder builder = new StringBuilder();
						int bytes = 0;
						do
						{
							bytes = stream.Read(data, 0, data.Length);
							builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
						} while (stream.DataAvailable);
						Console.WriteLine(builder.ToString());
						stream?.Close();
						client?.Close();
						break;
					}
				}
			});
			thread.Start();
			while (true)
			{
				Console.ReadLine();
			}
		}
	}
}
