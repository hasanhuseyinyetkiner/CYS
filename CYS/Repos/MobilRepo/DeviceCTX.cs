using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace CYS.Repos
{
	public class DeviceCTX
	{
		private readonly string connectionString = "Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;";

		private MySqlConnection GetConnection()
		{
			return new MySqlConnection(connectionString);
		}

		public List<Device> GetDevicesList(string sorgu, object param = null)
		{
			using (var connection = GetConnection())
			{
				return connection.Query<Device>(sorgu, param).ToList();
			}
		}

		public Device GetDeviceById(int id)
		{
			using (var connection = GetConnection())
			{
				return connection.QueryFirstOrDefault<Device>("SELECT * FROM device WHERE id = @id", new { id });
			}
		}

		public int AddDevice(Device device)
		{
			using (var connection = GetConnection())
			{
				const string query = @"INSERT INTO device (devicename, deviceguid, devicetype, active) VALUES (@devicename, @deviceguid, @devicetype, @active);";
				connection.Execute(query, device);
				return connection.QuerySingleOrDefault<int>("SELECT LAST_INSERT_ID();");
			}
		}

		public int UpdateDevice(Device device)
		{
			using (var connection = GetConnection())
			{
				const string query = @"UPDATE device SET devicename = @devicename, deviceguid = @deviceguid, devicetype = @devicetype, active = @active WHERE id = @id;";
				return connection.Execute(query, device);
			}
		}

		public int DeleteDevice(int id)
		{
			using (var connection = GetConnection())
			{
				const string query = @"DELETE FROM device WHERE id = @id;";
				return connection.Execute(query, new { id });
			}
		}
	}
}
