using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
    public class agirliksuCTX
    {
        private readonly string connectionString = "Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;";

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        public List<agirliksu> agirliksuList(string sorgu, object param)
        {
            using (var connection = GetConnection())
            {
                var list = connection.Query<agirliksu>($"{sorgu}", param).ToList();
                HayvanCTX hctx = new HayvanCTX();
                UserCTX uctx = new UserCTX();

                //foreach (var item in list)
                //{
                //    item.hayvan = hctx.hayvanTek("select * from Hayvan WHERE id = @id", new { id = item.hayvanId });
                //    item.user = uctx.userTek("SELECT * FROM user WHERE id = @id", new { id = item.userId });
                //}
                return list;
            }
        }

        public agirliksu agirliksuTek(string sorgu, object param)
        {
            using (var connection = GetConnection())
            {
                var item = connection.QueryFirstOrDefault<agirliksu>(sorgu, param);
                if (item != null)
                {
                    HayvanCTX hctx = new HayvanCTX();
                    UserCTX uctx = new UserCTX();

                    item.hayvan = hctx.hayvanTek("select * from Hayvan WHERE id = @id", new { id = item.hayvanId });
                    item.user = uctx.userTek("SELECT * FROM user WHERE id = @id", new { id = item.userId });
                }
                return item;
            }
        }

        public int agirliksuEkle(agirliksu kriter)
        {
            using (var connection = GetConnection())
            {
                const string query = @"
                    INSERT INTO agirliksu 
                    (userId, hayvanId, ilkOlcum, sonOlcum, tarih, reqestId, hayvangirdi, hayvancikti, hayvanui) 
                    VALUES 
                    (@userId, @hayvanId, @ilkOlcum, @sonOlcum, @tarih, @reqestId, @hayvangirdi, @hayvancikti, @hayvanui)";

                return connection.Execute(query, kriter);
            }
        }

        public int agirliksuGuncelle(agirliksu kriter)
        {
            using (var connection = GetConnection())
            {
                const string query = @"
                    UPDATE agirliksu 
                    SET userId = @userId, 
                        hayvanId = @hayvanId, 
                        ilkOlcum = @ilkOlcum, 
                        sonOlcum = @sonOlcum, 
                        tarih = @tarih, 
                        reqestId = @reqestId, 
                        hayvangirdi = @hayvangirdi, 
                        hayvancikti = @hayvancikti, 
                        hayvanui = @hayvanui 
                    WHERE id = @id";

                return connection.Execute(query, kriter);
            }
        }
    }
}
