using CYS.Data;
using CYS.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data;

namespace CYS.Repos
{
    public class TestWeightRepository : BaseRepository<TestWeightMeasurement>
    {
        private readonly IConfiguration _configuration;

        public TestWeightRepository(IConfiguration configuration) 
            : base(configuration, "TestWeightMeasurements")
        {
            _configuration = configuration;
        }

        public async Task<List<TestWeightMeasurement>> GetAllWeightsAsync()
        {
            using (var connection = GetConnection())
            {
                var result = await connection.QueryAsync<TestWeightMeasurement>(
                    "SELECT * FROM TestWeightMeasurements ORDER BY MeasurementDate DESC");
                return result.ToList();
            }
        }

        public async Task<TestWeightMeasurement> GetWeightByIdAsync(int id)
        {
            using (var connection = GetConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<TestWeightMeasurement>(
                    "SELECT * FROM TestWeightMeasurements WHERE Id = @Id", 
                    new { Id = id });
            }
        }

        public async Task<TestWeightMeasurement> AddWeightAsync(TestWeightMeasurement weight)
        {
            using (var connection = GetConnection())
            {
                weight.MeasurementDate = DateTime.Now;
                var sql = @"
                    INSERT INTO TestWeightMeasurements 
                    (AnimalId, Weight, Rfid, Notes, Source, MeasurementDate) 
                    VALUES 
                    (@AnimalId, @Weight, @Rfid, @Notes, @Source, @MeasurementDate);
                    SELECT LAST_INSERT_ID();";
                
                var id = await connection.ExecuteScalarAsync<int>(sql, weight);
                weight.Id = id;
                return weight;
            }
        }

        public async Task<bool> DeleteWeightAsync(int id)
        {
            using (var connection = GetConnection())
            {
                var rowsAffected = await connection.ExecuteAsync(
                    "DELETE FROM TestWeightMeasurements WHERE Id = @Id", 
                    new { Id = id });
                return rowsAffected > 0;
            }
        }

        public async Task ClearAllWeightsAsync()
        {
            using (var connection = GetConnection())
            {
                await connection.ExecuteAsync("DELETE FROM TestWeightMeasurements");
            }
        }
    }
} 