using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
	public class AgirlikHayvanCTX
	{
		public List<agirlikHayvan> agirlikHayvanList(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var list = connection.Query<agirlikHayvan>(sorgu, param).ToList();

				return list;
			}
		}

		public agirlikHayvan agirlikHayvanTek(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Query<agirlikHayvan>(sorgu, param).FirstOrDefault();

				return item;
			}
		}

		public int agirlikHayvanEkle(agirlikHayvan kriter)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Execute("insert into agirlikhayvan (hayvanId, agirlikId, requestId) values (@hayvanId, @agirlikId, @requestId)", kriter);
				return item;
			}
		}

		public int agirlikHayvanGuncelle(agirlikHayvan kriter)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Execute("update agirlikhayvan set hayvanId = @hayvanId,agirlikId=@agirlikId, requestId = @requestId isActive = @isActive where id = @id", kriter);
				return item;
			}
		}
	}
}
