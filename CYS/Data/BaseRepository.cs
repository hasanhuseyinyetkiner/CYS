using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Dapper;

namespace CYS.Data
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly IConfiguration _configuration;
        protected readonly string _tableName;

        protected BaseRepository(IConfiguration configuration, string tableName)
        {
            _configuration = configuration;
            _tableName = tableName;
        }

        public IDbConnection GetConnection()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            return new MySqlConnection(connectionString);
        }

        public virtual IEnumerable<T> GetAll()
        {
            using (var connection = GetConnection())
            {
                var query = $"SELECT * FROM {_tableName}";
                return connection.Query<T>(query);
            }
        }

        public virtual T GetById(int id)
        {
            using (var connection = GetConnection())
            {
                var query = $"SELECT * FROM {_tableName} WHERE id = @Id";
                return connection.QueryFirstOrDefault<T>(query, new { Id = id });
            }
        }

        public virtual void Add(T entity)
        {
            throw new NotImplementedException("Add method must be implemented in derived repository");
        }

        public virtual void Update(T entity)
        {
            throw new NotImplementedException("Update method must be implemented in derived repository");
        }

        public virtual void Delete(int id)
        {
            using (var connection = GetConnection())
            {
                var query = $"DELETE FROM {_tableName} WHERE id = @Id";
                connection.Execute(query, new { Id = id });
            }
        }
    }
} 