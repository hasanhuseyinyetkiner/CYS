using System;

namespace CYS.Models
{
	public class UserModule
	{
		public int id { get; set; }
		public int userid { get; set; }
		public int moduleid { get; set; }
		public int aktif { get; set; }

		// Related entity
		public Modules module { get; set; }
		public User user { get; set; }
	}
}
