using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CYS.Repos
{
	public class HayvanMobilCTX
	{
		private readonly string connectionString = "Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;";

		private MySqlConnection GetConnection()
		{
			return new MySqlConnection(connectionString);
		}

		public List<Hayvan> hayvanList(string sorgu, object param)
		{
			using (var connection = GetConnection())
			{
				var list = connection.Query<Hayvan>(sorgu, param).ToList();
				var uctx = new UserCTX();
				var kctx = new KategoriCTX();
				var hkuCTX = new HayvanKriterUnsurCTX();

				foreach (var item in list)
				{
					item.user = uctx.userTek("SELECT * FROM user WHERE id = @id", new { id = item.userId });
					item.kategori = kctx.KategoriTek("SELECT * FROM kategori WHERE id = @id", new { id = item.kategoriId });
					item.ozellikler = hkuCTX.HayvanKriterUnsurList(
						"SELECT * FROM hayvankriterunsur WHERE hayvanId = @hayvanId AND aktif = 1",
						new { hayvanId = item.id }
					);
				}
				return list;
			}
		}

		public List<Hayvan> hayvanListSadece(string sorgu, object param)
		{
			using (var connection = GetConnection())
			{
				return connection.Query<Hayvan>(sorgu, param).ToList();
			}
		}

		public Hayvan hayvanTek(string sorgu, object param)
		{
			using (var connection = GetConnection())
			{
				var item = connection.QueryFirstOrDefault<Hayvan>(sorgu, param);
				if (item != null)
				{
					var uctx = new UserCTX();
					var kctx = new KategoriCTX();
					var hkuCTX = new HayvanKriterUnsurCTX();

					item.user = uctx.userTek("SELECT * FROM user WHERE id = @id", new { id = item.userId });
					item.kategori = kctx.KategoriTek("SELECT * FROM kategori WHERE id = @id", new { id = item.kategoriId });
					item.ozellikler = hkuCTX.HayvanKriterUnsurList(
						"SELECT * FROM hayvankriterunsur WHERE hayvanId = @hayvanId AND aktif = 1",
						new { hayvanId = item.id }
					);
				}
				return item;
			}
		}

		public Hayvan hayvanTekSadece(string sorgu, object param)
		{
			using (var connection = GetConnection())
			{
				return connection.QueryFirstOrDefault<Hayvan>(sorgu, param);
			}
		}

		public int hayvanEkle(Hayvan hayvan)
		{
			using (var connection = GetConnection())
			{
				const string insertQuery = @"
                    INSERT INTO hayvan 
                    (hayvanguid, rfid, ilkdogumagirligi, suttenkesimagirligi, mevcutagirlik, kategoriid, 
                     cinsiyet, kupeismi, userid, eklenmezamani, aktif) 
                    VALUES 
                    (@hayvanguid, @rfid, @ilkdogumagirligi, @suttenkesimagirligi, @mevcutagirlik, 
                     @kategoriid, @cinsiyet, @kupeismi, @userid, @eklenmezamani, @aktif);";

				connection.Execute(insertQuery, hayvan);

				const string selectQuery = "SELECT LAST_INSERT_ID();";
				return connection.QuerySingleOrDefault<int>(selectQuery);
			}
		}

		public int hayvanGuncelle(Hayvan hayvan)
		{
			hayvan.sonGuncelleme = DateTime.Now;

			using (var connection = GetConnection())
			{
				const string updateQuery = @"
                    UPDATE hayvanmobil 
                    SET hayvanguid = @hayvanguid, 
                        rfid = @rfid, 
                        ilkdogumagirligi = @ilkdogumagirligi, 
                        suttenkesimagirligi = @suttenkesimagirligi, 
                        mevcutagirlik = @mevcutagirlik, 
                        kategoriid = @kategoriid, 
                        cinsiyet = @cinsiyet, 
                        kupeismi = @kupeismi, 
                        userid = @userid, 
                        aktif = @aktif, 
                        ensonguncelleme = @ensonguncelleme 
                    WHERE id = @id";

				return connection.Execute(updateQuery, hayvan);
			}
		}
	}
}
