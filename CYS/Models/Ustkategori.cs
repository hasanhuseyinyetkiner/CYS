namespace CYS.Models
{
    public class Ustkategori
    {
        public int id { get; set; }
        public string name { get; set; }
        public int isActive { get; set; }
        public List<Kategori> kategoriler { set; get; }

    }
}
