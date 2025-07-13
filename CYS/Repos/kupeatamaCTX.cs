using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
	public class kupeatamaCTX
	{
		public List<KupeAtama> kupeAtamaOlcumList(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var list = connection.Query<KupeAtama>(sorgu, param).ToList();
				UserCTX uctx = new UserCTX();
				foreach (var item in list)
				{
					item.user = uctx.userTek("select * from user where id = @id", new { id = item.userId });
				}
				return list;
			}
		}

		public KupeAtama kupeAtamaTek(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Query<KupeAtama>(sorgu, param).FirstOrDefault();
				
				UserCTX uctx = new UserCTX();
				if (item != null)
					item.user = uctx.userTek("select * from user where id = @id", new { id = item.userId });
				return item;
			}
		}

		public int kupeAtamaEkle(KupeAtama hayvan)
		{
			hayvan.tarih = DateTime.Now;
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Execute("insert into kupeatama ( userId, kupeRfid, tarih, requestId) values (@userId, @kupeRfid, @tarih, @requestId)", hayvan);
				return item;
			}
		}

		public int kupeAtamaGuncelle(KupeAtama hayvan)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Execute("update  kupeatama set userId = @userId,kupeRfid=@kupeRfid, aktif = @aktif, requestId = @requestId where id = @id", hayvan);
				return item;
			}
		}
	}
}
