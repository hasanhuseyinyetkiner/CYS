using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
	public class cihazCTX
	{
		public List<cihaz> cihazList(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var list = connection.Query<cihaz>(sorgu, param).ToList();

				return list;
			}
		}

		public cihaz cihazTek(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Query<cihaz>(sorgu, param).FirstOrDefault();

				return item;
			}
		}

		public int cihazEkle(cihaz kriter)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Execute("insert into cihaz (cihazId, cihazAdi, cihazIp, aktif) values (@cihazId, @cihazAdi, @cihazIp, @aktif)", kriter);
				return item;
			}
		}

		public int cihazGuncelle(processtype kriter)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Execute("update cihaz set cihazId = @cihazId, cihazAdi= @cihazAdi, cihazIp = @cihazIp, aktif = @aktif where id = @id", kriter);
				return item;
			}
		}
	}
}
