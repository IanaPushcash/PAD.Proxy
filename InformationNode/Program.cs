using System;

namespace InformationNode
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Enter file path:");
			var filePath = Console.ReadLine();
			//List<Person> l = new List<Person>();
			//l.Add(new Person("Iana", 20, "Moldova"));
			//l.Add(new Person("Anna", 29, "Russia"));
			//l.Add(new Person("Victor", 12, "Germania"));
			//l.Add(new Person("Artur", 18, "Suedia"));
			//l.Add(new Person("Cristi", 15, "Moldova"));
			//l.Add(new Person("Dumitru", 22, "Romania"));
			//File.WriteAllText(filePath, JsonConvert.SerializeObject(l));
			Console.WriteLine("Enter Port:");
			var port = Convert.ToInt32(Console.ReadLine());
			Console.WriteLine("Count of linked nodes:");
			var countPorts = Convert.ToInt32(Console.ReadLine());
			var node = new Node(filePath, port, "127.0.0.1");
			for (int i = 0; i < countPorts; i++)
			{
				Console.WriteLine("Enter port for {0} node", i+1);
				var nNode = new Node(Convert.ToInt32(Console.ReadLine()), "127.0.0.1");
				node.SendConnect(nNode);
				node.MyNodes.Add(nNode);

			}
			node.Start();
			while (true)
			{
				Console.ReadLine();
			}
		}
	}
}
