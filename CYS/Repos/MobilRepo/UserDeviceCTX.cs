using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace CYS.Repos
{
	public class UserDeviceCTX
	{
		private readonly string connectionString = "Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;";

		private MySqlConnection GetConnection()
		{
			return new MySqlConnection(connectionString);
		}

		public List<UserDevice> GetUserDevicesList(string sorgu, object param = null)
		{
			using (var connection = GetConnection())
			{
				var userDeviceList = connection.Query<UserDevice>(sorgu, param).ToList();
				var deviceCtx = new DeviceCTX();
				var userCtx = new UserCTX();

				foreach (var userDevice in userDeviceList)
				{
					userDevice.device = deviceCtx.GetDeviceById(userDevice.deviceid);
					userDevice.user = userCtx.userTek("SELECT * FROM user WHERE id = @id", new { id = userDevice.userid });
				}
				return userDeviceList;
			}
		}

		public UserDevice GetUserDeviceById(int id)
		{
			using (var connection = GetConnection())
			{
				var userDevice = connection.QueryFirstOrDefault<UserDevice>("SELECT * FROM userdevice WHERE id = @id", new { id });
				if (userDevice != null)
				{
					var deviceCtx = new DeviceCTX();
					var userCtx = new UserCTX();

					userDevice.device = deviceCtx.GetDeviceById(userDevice.deviceid);
					userDevice.user = userCtx.userTek("SELECT * FROM user WHERE id = @id", new { id = userDevice.userid });
				}
				return userDevice;
			}
		}

		public int AddUserDevice(UserDevice userDevice)
		{
			using (var connection = GetConnection())
			{
				const string query = @"INSERT INTO userdevice (deviceid, userid, active) VALUES (@deviceid, @userid, @active);";
				connection.Execute(query, userDevice);
				return connection.QuerySingleOrDefault<int>("SELECT LAST_INSERT_ID();");
			}
		}

		public int UpdateUserDevice(UserDevice userDevice)
		{
			using (var connection = GetConnection())
			{
				const string query = @"UPDATE userdevice SET deviceid = @deviceid, userid = @userid, active = @active WHERE id = @id;";
				return connection.Execute(query, userDevice);
			}
		}

		public int DeleteUserDevice(int id)
		{
			using (var connection = GetConnection())
			{
				const string query = @"DELETE FROM userdevice WHERE id = @id;";
				return connection.Execute(query, new { id });
			}
		}
	}
}
