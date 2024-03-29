using DilkashDBMS.DAL.Models;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace DilkashDBMS.DAL
{
    public class FoodAdoNetRepository : IFoodRepository
    {
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
            cmd.CommandText = "select * from Food";
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
                    CreatedAt = rdr.GetDateTime("CreatedAt"),
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
            cmd.CommandText = @"select * from Food
                                where FoodId = @FoodId";

            var pId = cmd.CreateParameter();
            pId.ParameterName = "@FoodId";
            pId.Value = id;
            pId.DbType = DbType.Int32;
            pId.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(pId);

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
                    CreatedAt =  rdr.GetFieldValue<DateTime>("CreatedAt"),
                };
            }

            return null;
        }

        public int Insert(Food food)
        {
            using var conn = new SqlConnection(_connStr);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"insert into Food(FoodName, FoodDescription, FoodImage, FoodType, Availability, Price, CreatedAt)
                                output inserted.FoodId
                                values (@FoodName, @FoodDescription, @FoodImage, @FoodType, @Availability, @Price, @CreatedAt)";
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
            cmd.CommandText = "udpUpdateEmployee";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FirstName", emp.FirstName);
            cmd.Parameters.AddWithValue("@LastName", emp.LastName);
            cmd.Parameters.AddWithValue("@ProfilePicture", emp.ProfilePicture ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Position", emp.Position);
            cmd.Parameters.AddWithValue("@Salary", emp.Salary ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ContactNumber", emp.ContactNumber ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@HireDate", emp.HireDate);
            cmd.Parameters.AddWithValue("@IsActive", emp.IsActive);
            cmd.Parameters.AddWithValue("@EmployeeID", emp.EmployeeID);

            conn.Open();
            int updatedCount = cmd.ExecuteNonQuery();
            if (updatedCount == 0)
                throw new Exception($"Employee does not exists!, {emp.EmployeeID}");
        }
        public void Delete(int id)
        {
            using var conn=new SqlConnection(_connStr);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"delete from Food where FoodId=@FoodId ";
            cmd.Parameters.AddWithValue("@FoodId",id);
            conn.Open( );
            int updatedCount = cmd.ExecuteNonQuery();
            if (updatedCount==0) {
                    throw new Exception($"Food does not exists,id={id}");            
            }
                
        }
    }
}
