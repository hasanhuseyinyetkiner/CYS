using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
	public class processsettingCTX
	{
		public List<processsetting> processsettingList(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var list = connection.Query<processsetting>(sorgu, param).ToList();

				return list;
			}
		}

		public processsetting processsettingTek(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Query<processsetting>(sorgu, param).FirstOrDefault();

				return item;
			}
		}

		public int processsettingEkle(processsetting kriter)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Execute("insert into processsetting ( mevcutRequest, oncekiIslem, mevcutIslem, guncellemeZamani, islemModu) values (@mevcutRequest, @oncekiIslem, @mevcutIslem, @guncellemeZamani, @islemModu)", kriter);
				return item;
			}
		}

		public int processtypeGuncelle(processsetting kriter)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Execute("update processsetting set  mevcutRequest = @mevcutRequest, oncekiIslem = @oncekiIslem, mevcutIslem = @mevcutIslem, guncellemeZamani = @guncellemeZamani, islemModu = @islemModu where id = 1", kriter);
				return item;
			}
		}
	}
}
