using System.Collections.Generic;

namespace Mediator.Messages
{
	class Filter
	{
		public Person FilterPerson { get; set; } 
	}

	class Sort
	{
		public List<string> Fields { get; set; }
		public bool IsAsc { get; set; }
	}
}
