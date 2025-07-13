using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
	public class olcumSessionCTX
	{
		public List<olcumSession> olcumSessionList(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var list = connection.Query<olcumSession>(sorgu, param).ToList();

				return list;
			}
		}

		public olcumSession olcumTek(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Query<olcumSession>(sorgu, param).FirstOrDefault();

				return item;
			}
		}

		public int insert(olcumSession ol)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Execute("insert into olcumsession (sessionGuid, tarih) values (@sessionGuid, @tarih)", ol);
				return item;
			}
		}

		public int update(olcumSession ol)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Execute("update  olcumsession set sessionGuid = @sessionGuid,tarih=@tarih where id = @id", ol);
				return item;
			}
		}
	}
}
