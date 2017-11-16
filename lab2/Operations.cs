using System.Collections.Generic;

namespace lab2
{
	public class Filter
	{
		public Person FilterPerson { get; set; }
	}

	public class Sort
	{
		public List<string> Fields { get; set; }
		public bool IsAsc { get; set; }
	}
}
