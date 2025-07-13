using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
    public class HayvanCTX
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
                    item.ozellikler = hkuCTX.HayvanKriterUnsurList("select * from hayvankriterunsur WHERE hayvanId = @hayvanId AND isActive = 1", new { hayvanId = item.id });
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
                    item.ozellikler = hkuCTX.HayvanKriterUnsurList("select * from hayvankriterunsur WHERE hayvanId = @hayvanId AND isActive = 1", new { hayvanId = item.id });
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
			if (hayvan.agirlik == null)
				hayvan.agirlik = "0";

			using (var connection = GetConnection())
			{
				// İlk sorgu: INSERT işlemini gerçekleştirir
				const string insertQuery = @"
			insert into Hayvan 
			(rfidKodu, kupeIsmi, cinsiyet, agirlik, userId, kategoriId, requestId, ilkdogumagirligi, suttenkesimagirligi) 
			VALUES 
			(@rfidKodu, @kupeIsmi, @cinsiyet, @agirlik, @userId, @kategoriId, @requestId, @ilkdogumagirligi, @suttenkesimagirligi);";

				// INSERT işlemi
				connection.Execute(insertQuery, hayvan);

				// İkinci sorgu: Son eklenen ID'yi almak için
				const string selectQuery = "SELECT LAST_INSERT_ID();";

				// Son eklenen ID'yi döndürür
				return connection.QuerySingleOrDefault<int>(selectQuery);
			}
		}

        public int hayvanGuncelle(Hayvan hayvan)
        {
            hayvan.sonGuncelleme = DateTime.Now;
            using (var connection = GetConnection())
            {
                const string query = @"
					UPDATE hayvan 
					SET rfidKodu = @rfidKodu, 
						kupeIsmi = @kupeIsmi, 
						cinsiyet = @cinsiyet, 
						agirlik = @agirlik, 
						userId = @userId, 
						kategoriId = @kategoriId, 
						aktif = @aktif, 
						requestId = @requestId ,
                        ilkdogumagirligi= @ilkdogumagirligi , 
                        suttenkesimagirligi = @suttenkesimagirligi
					WHERE id = @id";

                return connection.Execute(query, hayvan);
            }
        }

        // executeNonQuery metodu
        public int executeNonQuery(string sorgu, object param)
        {
            using (var connection = GetConnection())
            {
                return connection.Execute(sorgu, param);
            }
        }
    }
}
