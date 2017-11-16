using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace lab2
{
	class Program
	{
		static void Main(string[] args)
		{
			var messages = new List<Message>()
			{
				new Message()
				{
					Author = "I",
					Body = "All",
					Type = "GetInfo",
					ReturnJson = true,
					FilterBy = new Filter() {FilterPerson = new Person(null, 18, null)}
				},
				new Message()
				{
					Author = "I",
					Body = "All",
					Type = "GetInfo",
					ReturnJson = false,
					SortBy = new Sort() {Fields = new List<string>() {"Country"}}
				}
			};

			Thread thread = new Thread(() =>
			{
				foreach (var x in messages)
				{
					Console.WriteLine(JsonConvert.SerializeObject(x));
					TcpClient client = new TcpClient("127.0.0.1", 11000);
					var stream = client.GetStream();
					byte[] data = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(x));
					// отправка сообщения
					stream.Write(data, 0, data.Length);
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
							var response = JsonConvert.DeserializeObject<Message>(builder.ToString());
							if (response.ReturnJson && Verifier.VerifyJson(response.Body))
							{
								Console.WriteLine($"\n Collected data: {response.Body}");
								JsonConvert.DeserializeObject<List<Person>>(response.Body).ForEach(Console.WriteLine);
							}
							else if (!response.ReturnJson && Verifier.VerifyXml(response.Body))
							{
								XmlSerializer formatter = new XmlSerializer(typeof(List<Person>));
								using (StringReader textReader = new StringReader(response.Body))
								{
									Console.WriteLine($"\n Collected data: {response.Body}");
									((List<Person>)formatter.Deserialize(textReader)).ForEach(Console.WriteLine);
								}
							}

							stream?.Close();
							client?.Close();
							break;
						}
					}
					Console.WriteLine("\n");
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
