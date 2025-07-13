using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
	public class kupehayvanCTX
	{
		public List<kupehayvan> kupehayvanList(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var list = connection.Query<kupehayvan>(sorgu, param).ToList();

				return list;
			}
		}

		public kupehayvan kupehayvanTek(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Query<kupehayvan>(sorgu, param).FirstOrDefault();

				return item;
			}
		}

		public int kupehayvanEkle(kupehayvan kriter)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Execute("insert into kupehayvan (hayvanId, kupeId, requestId) values (@hayvanId, @kupeId, @requestId)", kriter);
				return item;
			}
		}

		public int kupehayvanGuncelle(kupehayvan kriter)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Execute("update kupehayvan set hayvanId = @hayvanId,kupeId=@kupeId, isActive = @isActive, requestId = @requestId where id = @id", kriter);
				return item;
			}
		}
	}
}
