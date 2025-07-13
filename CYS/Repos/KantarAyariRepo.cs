using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
	public class KantarAyariRepo
	{
		public List<KantarAyari> kantarAyariList(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var list = connection.Query<KantarAyari>(sorgu, param).ToList();
				
				return list;
			}
		}

		public KantarAyari KantarAyariTek(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Query<KantarAyari>(sorgu, param).FirstOrDefault();
				
				return item;
			}
		}

		public int KantarAyariEkle(KantarAyari kriter)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Execute("insert into kantarayari (requestId, tare3, tare4, tare5, esikAgirlik, tarih) values (@requestId, @tare3, @tare4, @tare5, @esikAgirlik, @tarih)", kriter);
				return item;
			}
		}

		public int KantarAyariGuncelle(KantarAyari kriter)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Execute("update kantarayari set requestId = @requestId, tare3 = @tare3, tare4 = @tare4, tare5 = @tare5, esikAgirlik = @esikAgirlik, tarih = @tarih where id = @id", kriter);
				return item;
			}
		}
	}
}
