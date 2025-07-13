using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace CYS.Repos
{
	public class ModulesCTX
	{
		private readonly string connectionString = "Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;";

		private MySqlConnection GetConnection()
		{
			return new MySqlConnection(connectionString);
		}

		public List<Modules> GetModulesList(string sorgu, object param = null)
		{
			using (var connection = GetConnection())
			{
				return connection.Query<Modules>(sorgu, param).ToList();
			}
		}

		public Modules GetModuleById(int id)
		{
			using (var connection = GetConnection())
			{
				return connection.QueryFirstOrDefault<Modules>("SELECT * FROM modules WHERE id = @id", new { id });
			}
		}

		public int AddModule(Modules module)
		{
			using (var connection = GetConnection())
			{
				const string query = @"INSERT INTO modules (modulename, ispublic) VALUES (@modulename, @ispublic);";
				connection.Execute(query, module);
				return connection.QuerySingleOrDefault<int>("SELECT LAST_INSERT_ID();");
			}
		}

		public int UpdateModule(Modules module)
		{
			using (var connection = GetConnection())
			{
				const string query = @"UPDATE modules SET modulename = @modulename, ispublic = @ispublic WHERE id = @id;";
				return connection.Execute(query, module);
			}
		}

		public int DeleteModule(int id)
		{
			using (var connection = GetConnection())
			{
				const string query = @"DELETE FROM modules WHERE id = @id;";
				return connection.Execute(query, new { id });
			}
		}
	}
}
