using System;

namespace CYS.Models
{
	public class UserDevice
	{
		public int id { get; set; }
		public int deviceid { get; set; }
		public int userid { get; set; }
		public int active { get; set; }

		// Related entities
		public Device device { get; set; }
		public User user { get; set; }
	}
}
