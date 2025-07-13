using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace CYS.Repos
{
	public class UserModuleCTX
	{
		private readonly string connectionString = "Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;";

		private MySqlConnection GetConnection()
		{
			return new MySqlConnection(connectionString);
		}

		public List<UserModule> GetUserModulesList(string sorgu, object param = null)
		{
			using (var connection = GetConnection())
			{
				var userModuleList = connection.Query<UserModule>(sorgu, param).ToList();
				var modulesCtx = new ModulesCTX();
				var userCtx = new UserCTX();

				foreach (var userModule in userModuleList)
				{
					userModule.module = modulesCtx.GetModuleById(userModule.moduleid);
					userModule.user = userCtx.userTek("SELECT * FROM user WHERE id = @id", new { id = userModule.userid });
				}
				return userModuleList;
			}
		}

		public UserModule GetUserModuleById(int id)
		{
			using (var connection = GetConnection())
			{
				var userModule = connection.QueryFirstOrDefault<UserModule>("SELECT * FROM usermodule WHERE id = @id", new { id });
				if (userModule != null)
				{
					var modulesCtx = new ModulesCTX();
					var userCtx = new UserCTX();

					userModule.module = modulesCtx.GetModuleById(userModule.moduleid);
					userModule.user = userCtx.userTek("SELECT * FROM user WHERE id = @id", new { id = userModule.userid });
				}
				return userModule;
			}
		}

		public int AddUserModule(UserModule userModule)
		{
			using (var connection = GetConnection())
			{
				const string query = @"INSERT INTO usermodule (userid, moduleid, aktif) VALUES (@userid, @moduleid, @aktif);";
				connection.Execute(query, userModule);
				return connection.QuerySingleOrDefault<int>("SELECT LAST_INSERT_ID();");
			}
		}

		public int UpdateUserModule(UserModule userModule)
		{
			using (var connection = GetConnection())
			{
				const string query = @"UPDATE usermodule SET userid = @userid, moduleid = @moduleid, aktif = @aktif WHERE id = @id;";
				return connection.Execute(query, userModule);
			}
		}

		public int DeleteUserModule(int id)
		{
			using (var connection = GetConnection())
			{
				const string query = @"DELETE FROM usermodule WHERE id = @id;";
				return connection.Execute(query, new { id });
			}
		}
	}
}
