namespace lab2
{
	public class Message
	{
		public string Author { get; set; }
		public string Type { get; set; }
		public string Body { get; set; }
		
		public Filter FilterBy { get; set; }
		public Sort SortBy { get; set; }

		public bool ReturnJson { get; set; }
	}
}
