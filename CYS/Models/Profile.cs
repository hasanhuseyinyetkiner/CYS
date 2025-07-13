namespace CYS.Models
{
	public class Profile
	{
		public int id { get; set; }
		public int userId { get; set; }
		public User user { get; set; }
		public string companyName { get; set; }
		public string companyDescription { get; set; }
		public int companyId { get; set; }
		public string address { get; set; }
		public string phoneNumber { get; set; }
		public string cellPhoneNumber { get; set; }
		public byte[] logo { get; set; }
		public string cihazLink { get; set; }



	}
}
