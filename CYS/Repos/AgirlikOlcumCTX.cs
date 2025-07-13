using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CYS.Repos
{
	public class AgirlikOlcumCTX
	{
		private readonly string _connectionString;

		public AgirlikOlcumCTX()
		{
			// appsettings.json'dan bağlantı dizesini al
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();

			_connectionString = configuration.GetConnectionString("DefaultConnection");
		}

		public List<AgirlikOlcum> agirlikOlcumList(string sorgu, object param)
		{
			using (var connection = new MySqlConnection(_connectionString))
			{
				var list = connection.Query<AgirlikOlcum>(sorgu, param).ToList();
				UserCTX uctx = new UserCTX();
				foreach (var item in list)
				{
					item.user = uctx.userTek("select * from user where id = @id", new { id = item.userId});
				}
				return list;
			}
		}

		public AgirlikOlcum agirlikOlcumTek(string sorgu, object param)
		{
			using (var connection = new MySqlConnection(_connectionString))
			{
				var item = connection.Query<AgirlikOlcum>(sorgu, param).FirstOrDefault();
				UserCTX uctx = new UserCTX();
				if (item != null)
					item.user = uctx.userTek("select * from user where id = @id", new { id = item.userId });
				return item;
			}
		}

		public int agirlikOlcumEkle(AgirlikOlcum hayvan)
		{
			using (var connection = new MySqlConnection(_connectionString))
			{
				var item = connection.Execute("insert into agirlikolcum (userId,agirlikOlcumu, requestId, aktif) values (@userId,@agirlikOlcumu, @requestId, @aktif)", hayvan);
				return item;
			}
		}

		public int agirlikOlcumGuncelle(AgirlikOlcum hayvan)
		{
			using (var connection = new MySqlConnection(_connectionString))
			{
				var item = connection.Execute("update agirlikolcum set userId = @userId,agirlikOlcumu=@agirlikOlcumu, aktif = @aktif, requestId = @requestId where id = @id", hayvan);
				return item;
			}
		}
	}
}
