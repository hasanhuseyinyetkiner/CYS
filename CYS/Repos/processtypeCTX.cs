using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
	public class processtypeCTX
	{
		public List<processtype> processtypeList(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var list = connection.Query<processtype>(sorgu, param).ToList();

				return list;
			}
		}

		public processtype processtypeTek(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Query<processtype>(sorgu, param).FirstOrDefault();

				return item;
			}
		}

		public int processtypeEkle(processtype kriter)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Execute("insert into processtype ( processAdi, processKodu) values (@processAdi, @processKodu)", kriter);
				return item;
			}
		}

		public int processtypeGuncelle(processtype kriter)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Execute("update processtype set processAdi = @processAdi,processKodu=@processKodu where id = @id", kriter);
				return item;
			}
		}
	}
}
