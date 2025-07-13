using CYS.Models.Mobil.CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos.MobilRepo
{
	public class UserSessionCTX
	{
		private readonly string connectionString = "Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;";

		private MySqlConnection GetConnection()
		{
			return new MySqlConnection(connectionString);
		}

		public List<UserSession> userSessionList(string sorgu, object param)
		{
			using (var connection = GetConnection())
			{
				var list = connection.Query<UserSession>(sorgu, param).ToList();
				var uctx = new UserCTX();

				foreach (var item in list)
				{
					item.user = uctx.userTek("SELECT * FROM user WHERE id = @id", new { id = item.userid });
				}
				return list;
			}
		}

		public List<UserSession> userSessionListSadece(string sorgu, object param)
		{
			using (var connection = GetConnection())
			{
				return connection.Query<UserSession>(sorgu, param).ToList();
			}
		}

		public UserSession userSessionTek(string sorgu, object param)
		{
			using (var connection = GetConnection())
			{
				var item = connection.QueryFirstOrDefault<UserSession>(sorgu, param);
				if (item != null)
				{
					var uctx = new UserCTX();
					item.user = uctx.userTek("SELECT * FROM user WHERE id = @id", new { id = item.userid });
				}
				return item;
			}
		}

		public UserSession userSessionTekSadece(string sorgu, object param)
		{
			using (var connection = GetConnection())
			{
				return connection.QueryFirstOrDefault<UserSession>(sorgu, param);
			}
		}

		public int userSessionEkle(UserSession userSession)
		{
			using (var connection = GetConnection())
			{
				const string insertQuery = @"
                    INSERT INTO usersession 
                    (userid, devicename, deviceguid, devicekey, sessiontimeout, ipaddress) 
                    VALUES 
                    (@userid, @devicename, @deviceguid, @devicekey, @sessiontimeout, @ipaddress);";

				connection.Execute(insertQuery, userSession);

				const string selectQuery = "SELECT LAST_INSERT_ID();";
				return connection.QuerySingleOrDefault<int>(selectQuery);
			}
		}

		public int userSessionGuncelle(UserSession userSession)
		{
			using (var connection = GetConnection())
			{
				const string updateQuery = @"
                    UPDATE usersession 
                    SET userid = @userid, 
                        devicename = @devicename, 
                        deviceguid = @deviceguid, 
                        devicekey = @devicekey, 
                        sessiontimeout = @sessiontimeout, 
                        ipaddress = @ipaddress 
                    WHERE id = @id";

				return connection.Execute(updateQuery, userSession);
			}
		}
	}
	}
