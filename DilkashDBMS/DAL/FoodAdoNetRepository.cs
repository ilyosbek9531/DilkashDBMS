using DilkashDBMS.DAL.Models;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Dapper;

namespace DilkashDBMS.DAL
{
    public class FoodAdoNetRepository : IFoodRepository
    {
        private const string SQL_FILTER = @"SELECT FoodId, FoodName, FoodDescription, FoodImage, FoodType, Availability, Price, CreatedAt,
                                                COUNT(*) OVER () AS TotalCount
                                            FROM Food
                                            WHERE (FoodName LIKE COALESCE(@FoodName, '') + '%' OR @FoodName IS NULL)
                                                AND (CreatedAt = COALESCE(@CreatedAt, '1900-01-01') OR @CreatedAt IS NULL)
                                            ORDER BY {0}
                                            OFFSET (@PageNumber - 1) * @PageSize ROWS
                                            FETCH NEXT @PageSize ROWS ONLY";

        private readonly string _connStr;

        public FoodAdoNetRepository(string connStr)
        {
            _connStr = connStr;
        }

        public IList<Food> GetAll()
        {
            var list = new List<Food>();

            using var conn = new SqlConnection(_connStr);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "udpGetAllFoods";
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var food = new Food()
                {
                    FoodId = rdr.GetInt32("FoodId"),
                    FoodName = rdr.GetString("FoodName"),
                    FoodDescription = rdr.IsDBNull("FoodDescription") ? null : rdr.GetString("FoodDescription"),
                    FoodImage = rdr.IsDBNull(rdr.GetOrdinal("FoodImage")) ? null : (byte[])rdr["FoodImage"],
                    FoodType = rdr.GetString("FoodType"),
                    Availability = rdr.GetBoolean("Availability"),
                    Price = rdr.GetInt32("Price"),
                    CreatedAt = rdr.IsDBNull("CreatedAt") ? null : rdr.GetDateTime("CreatedAt"),
                };
                list.Add(food);
            }

            return list;
        }

        public async IAsyncEnumerable<Food> GetAllAsync()
        {
            using var conn = new SqlConnection(_connStr);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "select * from Food";
            await conn.OpenAsync();
            using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                yield return new Food()
                {
                    FoodId = await rdr.GetFieldValueAsync<int>("FoodId"),
                    FoodName = await rdr.GetFieldValueAsync<string>("FoodName"),
                    FoodDescription = await rdr.IsDBNullAsync("FoodDescription") ? null : await rdr.GetFieldValueAsync<string>("FoodDescription"),
                    FoodImage = await rdr.IsDBNullAsync(rdr.GetOrdinal("FoodImage")) ? null : (byte[])rdr["FoodImage"],
                    FoodType = await rdr.GetFieldValueAsync<string>("FoodType"),
                    Availability = await rdr.GetFieldValueAsync<bool>("Availability"),
                    Price = await rdr.GetFieldValueAsync<int>("Price"),
                    CreatedAt = await rdr.GetFieldValueAsync<DateTime>("CreatedAt"),
                };
            }
        }

        public Food GetById(int id)
        {
            using var conn = new SqlConnection(_connStr);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "udpGetFoodById";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FoodId", id);

            conn.Open();
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                return new Food()
                {
                    FoodId = rdr.GetFieldValue<int>("FoodId"),
                    FoodName = rdr.GetFieldValue<string>("FoodName"),
                    FoodDescription = rdr.IsDBNull("FoodDescription") ? null :  rdr.GetFieldValue<string>("FoodDescription"),
                    FoodImage =  rdr.IsDBNull("FoodImage") ? null :  rdr.GetFieldValue<byte[]>("FoodImage"),
                    FoodType =  rdr.GetFieldValue<string>("FoodType"),
                    Availability =  rdr.GetFieldValue<bool>("Availability"),
                    Price =  rdr.GetFieldValue<int>("Price"),
                    CreatedAt = rdr.IsDBNull("CreatedAt") ? null : rdr.GetFieldValue<DateTime>("CreatedAt"),
                };
            }

            return null;
        }

        public int Insert(Food food)
        {
            using var conn = new SqlConnection(_connStr);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "udpInsertFood";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FoodName", food.FoodName);
            cmd.Parameters.AddWithValue("@FoodDescription", food.FoodDescription ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@FoodImage", (object)food.FoodImage ?? SqlBinary.Null);
            cmd.Parameters.AddWithValue("@FoodType", food.FoodType);
            cmd.Parameters.AddWithValue("@Availability", food.Availability);
            cmd.Parameters.AddWithValue("@Price", food.Price);
            cmd.Parameters.AddWithValue("@CreatedAt", food.CreatedAt ?? (object)DBNull.Value);
            conn.Open();
            int id = (int)cmd.ExecuteScalar();
            food.FoodId = id;
            return id;
        }

        public void Update(Food food)
        {

            using var conn = new SqlConnection(_connStr);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "udpUpdateFood";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FoodId", food.FoodId);
            cmd.Parameters.AddWithValue("@FoodName", food.FoodName);
            cmd.Parameters.AddWithValue("@FoodDescription", food.FoodDescription ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@FoodImage", (object)food.FoodImage ?? SqlBinary.Null);
            cmd.Parameters.AddWithValue("@FoodType", food.FoodType);
            cmd.Parameters.AddWithValue("@Availability", food.Availability);
            cmd.Parameters.AddWithValue("@Price", food.Price);
            cmd.Parameters.AddWithValue("@CreatedAt", food.CreatedAt ?? (object)DBNull.Value);

            conn.Open();
            int updatedCount = cmd.ExecuteNonQuery();
            if (updatedCount == 0)
                throw new Exception($"Food does not exists!, {food.FoodId}");
        }
        public void Delete(int id)
        {
            using var conn=new SqlConnection(_connStr);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "udpDeleteFood";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FoodId",id);
            conn.Open( );
            int updatedCount = cmd.ExecuteNonQuery();
            if (updatedCount==0) {
                    throw new Exception($"Food does not exists,id={id}");            
            }
                
        }

        public IEnumerable<Food> Filter(out int totalCount, string? foodName = null, DateTime? createdAt = null, string? sortColumn = "FoodId", bool sortDesc = false, int pageNumber = 1, int pageSize = 3)
        {
            if (pageNumber <= 0) pageNumber = 1;

            string orderBy = nameof(Food.FoodId);
            if (nameof(Food.FoodName).Equals(sortColumn))
                orderBy = nameof(Food.FoodName);

            string sql = string.Format(SQL_FILTER, $"{orderBy} {(sortDesc ? "DESC" : "ASC")}");

            using var conn = new SqlConnection(_connStr);
            var list = conn.Query<Food>(sql,
                new
                {
                    FoodName = foodName,
                    CreatedAt = createdAt,
                    PageNumber = pageNumber,
                    PageSize = pageSize

                }
                );
            totalCount = list?.FirstOrDefault()?.TotalCount ?? 0;
            return list ?? Enumerable.Empty<Food>();
        }
    }
}
