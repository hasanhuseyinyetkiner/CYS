namespace CYS.Models
{
    public class Kategori
    {
        public int id { get; set; }
        public int ustKategoriId { get; set; }
        public Ustkategori ustkategori { set; get; }
        public string kategoriAdi { get; set; }
        public byte[] resim { get; set; }
        public int isActive { get; set; }


    }
}
