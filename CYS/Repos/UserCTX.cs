using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
	public class UserCTX
	{
		public List<User> userList(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=;"))
			{
				var list = connection.Query<User>(sorgu, param).ToList();
				return list;
			}
		}

		public User userTek(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=;"))
			{
				var item = connection.Query<User>(sorgu, param).FirstOrDefault();
				return item;
			}
		}

		public int userEkle(Profile profil)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=;"))
			{
				var item = connection.Execute("insert into user (username, password) values (@userName, @password)", profil);
				return item;
			}
		}

		public int userGuncelle(Profile profil)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=;"))
			{
				var item = connection.Execute("update user set username = @username, password = @password where id = @id", profil);
				return item;
			}
		}
	}
}
