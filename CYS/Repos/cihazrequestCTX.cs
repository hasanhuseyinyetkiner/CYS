using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
	public class cihazrequestCTX
	{
		public List<cihazrequest> cihazrequestList(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var list = connection.Query<cihazrequest>(sorgu, param).ToList();

				return list;
			}
		}

		public cihazrequest cihazrequestTek(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Query<cihazrequest>(sorgu, param).FirstOrDefault();

				return item;
			}
		}

		public int cihazrequestEkle(cihazrequest kriter)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Execute("insert into cihazrequest (cihazId, requestUrl, requestResult, requestTime) values (@cihazId, @requestUrl, @requestResult, @requestTime)", kriter);
				return item;
			}
		}

		public int cihazrequestGuncelle(cihazrequest kriter)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;"))
			{
				var item = connection.Execute("update cihaz set cihazId = @cihazId, requestUrl= @requestUrl, requestResult= @requestResult, requestTime= @requestTime where id = @id", kriter);
				return item;
			}
		}
	}
}
