using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
	public class UstSoyCTX
	{
		public List<soyagaci> soyagaciList(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var list = connection.Query<soyagaci>(sorgu, param).ToList();
				HayvanCTX uctx = new HayvanCTX();
				foreach (var item in list)
				{
					item.ustHayvan = uctx.hayvanTek("select * from Hayvan where id = @id", new { id = item.ustHayvanId });
					item.hayvan = uctx.hayvanTek("select * from Hayvan where id = @id", new { id = item.hayvanId });

				}
				return list;
			}
		}

		public soyagaci soyagaciTek(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Query<soyagaci>(sorgu, param).FirstOrDefault();
				HayvanCTX uctx = new HayvanCTX();
				if (item != null)
				{
					item.ustHayvan = uctx.hayvanTek("select * from Hayvan where id = @id", new { id = item.ustHayvanId });
					item.hayvan = uctx.hayvanTek("select * from Hayvan where id = @id", new { id = item.hayvanId });
				}
				return item;
			}
		}

		public int soyagaciEkle(soyagaci soyagaci)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Execute("insert into soyagaci ( ustHayvanId, hayvanId) values (@ustHayvanId, @hayvanId)", soyagaci);
				return item;
			}
		}

		public int soyagaciGuncelle(soyagaci soyagaci)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Execute("update soyagaci set ustHayvanId = @ustHayvanId,hayvanId=@hayvanId, isActive = @isActive where id = @id", soyagaci);
				return item;
			}
		}
	}
}
