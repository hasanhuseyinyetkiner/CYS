using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
    public class KategoriCTX
    {
        public List<Kategori> KategoriList(string sorgu, object param)
        {
            using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
            {
                var list = connection.Query<Kategori>(sorgu, param).ToList();
                ustKategoriCTX uctx = new ustKategoriCTX();
                foreach(var item in list)
                    item.ustkategori = uctx.ustKategoriTek("select * from ustkategori where id = @id", new { id = item.ustKategoriId });

                return list;
            }
        }

        public Kategori KategoriTek(string sorgu, object param)
        {
            using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
            {
                var item = connection.Query<Kategori>(sorgu, param).FirstOrDefault();
                ustKategoriCTX uctx = new ustKategoriCTX();
                if(item != null)
                    item.ustkategori = uctx.ustKategoriTek("select * from ustkategori where id = @id", new { id = item.ustKategoriId });

                return item;
            }
        }

        public int KategoriEkle(Kategori kategori)
        {
            using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
            {
                var item = connection.Execute("insert into kategori (ustKategoriId, kategoriAdi, resim) values (@ustKategoriId, @kategoriAdi, @resim)", kategori);
                return item;
            }
        }

        public int KategoriGuncelle(Kategori kategori)
        {
            using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
            {
                var item = connection.Execute("update kategori set ustKategoriId = @ustKategoriId, kategoriAdi = @kategoriAdi, resim = @resim, isActive = @isActive where id = @id", kategori);
                return item;
            }
        }
    }
}
