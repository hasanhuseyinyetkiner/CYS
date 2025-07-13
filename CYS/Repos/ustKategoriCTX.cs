using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
    public class ustKategoriCTX
    {
        public List<Ustkategori> ustKategoriList(string sorgu, object param)
        {
            using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
            {
                var list = connection.Query<Ustkategori>(sorgu, param).ToList();
               
                return list;
            }
        }

        public Ustkategori ustKategoriTek(string sorgu, object param)
        {
            using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
            {
                var item = connection.Query<Ustkategori>(sorgu, param).FirstOrDefault();
               
                return item;
            }
        }

        public int ustKategoriEkle(Ustkategori ustkategori)
        {
            using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
            {
                var item = connection.Execute("insert into  ustkategori (name) values (@name)", ustkategori);
                return item;
            }
        }

        public int ustKategoriGuncelle(Ustkategori ustkategori)
        {
            using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
            {
                var item = connection.Execute("update  ustkategori set name = @name where id = @id", ustkategori);
                return item;
            }
        }
    }
}
