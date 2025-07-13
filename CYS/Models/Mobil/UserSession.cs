namespace CYS.Models.Mobil
{
	using System;

	namespace CYS.Models
	{
		public class UserSession
		{
			public int id { get; set; }
			public int userid { get; set; }
			public string devicename { get; set; }
			public string deviceguid { get; set; }
			public string devicekey { get; set; }
			public DateTime sessiontimeout { get; set; }
			public string ipaddress { get; set; }

			// Related entity
			public User user { get; set; }
		}
	}

}
