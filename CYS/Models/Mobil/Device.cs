using System;

namespace CYS.Models
{
	public class Device
	{
		public int id { get; set; }
		public string devicename { get; set; }
		public string deviceguid { get; set; }
		public int devicetype { get; set; }
		public int active { get; set; }
	}
}
