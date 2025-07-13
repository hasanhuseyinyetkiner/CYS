using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
	public class surecCTX
	{
		public List<surec> surecList(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=123456;"))
			{
				var list = connection.Query<surec>(sorgu, param).ToList();
				UserCTX uctx = new UserCTX();
				foreach(var item in list)
				{
					item.user = uctx.userTek("select * from user where id = @id", new {id = item.userId});
				}
				return list;
			}
				
		}

		public surec surecTek(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=123456;"))
			{
				var item = connection.Query<surec>(sorgu, param).FirstOrDefault();
				UserCTX uctx = new UserCTX();
				if(item != null)
					item.user = uctx.userTek("select * from user where id = @id", new { id = item.userId });

				return item;
			}
		}

		public int surecEkle(surec kategori)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=123456;"))
			{
				var item = connection.Execute("insert into surec (requestId, userId) values (@requestId, @userId)", kategori);
				return item;
			}
		}

		public int surecGuncelle(surec surec)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=123456;"))
			{
				var item = connection.Execute("update surec set requestId = @requestId, userId = @userId, tamamlandi = @tamamlandi, tamamlanmaTarihi = @tamamlanmaTarihi where id = @id", surec);
				return item;
			}
		}
	}
}
