using SampleWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace SampleWebApp.Services
{
    public interface IDatabaseService
    {
        IEnumerable<Animal> GetAnimals();

    }

    public interface IDatabaseService2
    {
        Task<IEnumerable<Animal>> GetAnimalsByStoredProcedureAsync(); // nieużywana
        Task<IEnumerable<Animal>> GetAnimalsAsync();
        Task<int> ChangeAnimalAsync(String ind, Animal am); // TAK
        Task<int> DeleteAnimalAsync(String ind);  // TAK
        Task<int> CheckIfExistsAsync(String id);  // TAK
        IEnumerable<Animal> AddAnimals(Animal am);

        // jak ma być async to musi być opakowany w task
    }
  

    public class DatabaseService : IDatabaseService
    {
        private IGetPass _pass;
        public DatabaseService(IGetPass pass)
        {
            _pass = pass;
        }
        public IEnumerable<Animal> GetAnimals()
        {
            
            using var con = new SqlConnection(_pass.GetPassword());

            using var com = new SqlCommand("select * from animal", con);
            con.Open();
            var dr = com.ExecuteReader();
            var result = new List<Animal>();
            while (dr.Read())
            {
                result.Add(new Animal
                {
                    Name = dr["Name"].ToString(),
                    Description = dr["Description"].ToString()
                });
            }

            return result;
        }
       
    }

    public class DatabaseService2 : IDatabaseService2
    {
        private IGetPass _pass;
        public DatabaseService2(IGetPass pass)
        {
            _pass = pass;
        }
        public async Task<int> ChangeAnimalAsync(String ind, Animal am)
        {
            String id = ind;
            // werfikacja czy jest co usuwać
            if (await CheckIfExistsAsync(id) == 2)
            {
                return 2;
            };
            using var con = new SqlConnection(_pass.GetPassword());
            // using var con = new SqlConnection("Data Source=.\\SQLExpress;Initial Catalog=zadanie6;Integrated Security=True");

            using var com = new SqlCommand();
            com.Connection = con;
            com.CommandText = "Update animal set Name=@param1, Description=@param2, Category=@param3, Area=@param4 WHERE IdAnimal=@paramID;";



            com.Parameters.AddWithValue("@paramID", ind);
            com.Parameters.AddWithValue("@param1", am.Name);
            com.Parameters.AddWithValue("@param2", am.Description);
            com.Parameters.AddWithValue("@param3", "Category");
            com.Parameters.AddWithValue("@param4", "Area");

            using var comSel = new SqlCommand();

            await con.OpenAsync();
             using DbTransaction tran = await con.BeginTransactionAsync();
            com.Transaction = (SqlTransaction)tran;
            try
            {

                com.Transaction = (SqlTransaction)tran;
                await com.ExecuteNonQueryAsync();
                com.Parameters.Clear();
                await tran.CommitAsync();
            }
            catch (SqlException exc)
            {
                //...
                await tran.RollbackAsync();
                return 0;
            }
            catch (Exception exc)
            {
                //...
                await tran.RollbackAsync();
                return 2;
            }

            return 1;
        }
        public async Task<int> DeleteAnimalAsync(String ind)
        {
            String id = ind;
            
            if (await CheckIfExistsAsync(id) == 2)
            {
                return 2;
            };
            using var con = new SqlConnection(_pass.GetPassword());
            using var com = new SqlCommand();
            com.Connection = con;
            com.CommandText = "Delete from animal WHERE IdAnimal=@paramID;";

            com.Parameters.AddWithValue("@paramID", ind);

            await con.OpenAsync();
            using DbTransaction tran = await con.BeginTransactionAsync();
            com.Transaction = (SqlTransaction)tran;
            try
            {
                await com.ExecuteNonQueryAsync();
                com.Parameters.Clear();
                await tran.CommitAsync();

            }
            catch (SqlException exc)
            {
                //...
                await tran.RollbackAsync();
                return 0;
            }
            catch (Exception exc)
            {
                //...
                await tran.RollbackAsync();
                return 0;
            }

            return 1;
        }
        // nieużywane
        public async Task<IEnumerable<Animal>> GetAnimalsAsync()
        {
            using var con = new SqlConnection("Data Source=.\\SQLExpress;Initial Catalog=zadanie6;Integrated Security=True");
            using var com = new SqlCommand("select * from animal", con);
            await con.OpenAsync();
            var dr = await com.ExecuteReaderAsync();
            var result = new List<Animal>();
            while (await dr.ReadAsync())
            {
                await Task.Delay(300);
                result.Add(new Animal
                {
                    Name = dr["Name"].ToString(),
                    Description = dr["Description"].ToString()
                });
            }

            return result;
        }
        // nieużywane
        public async Task<IEnumerable<Animal>> GetAnimalsByStoredProcedureAsync()
        {
            using var con = new SqlConnection("Data Source=.\\SQLExpress;Initial Catalog=zadanie6;Integrated Security=True");
            using var com = new SqlCommand("GetAnimals", con);
            com.CommandType = CommandType.StoredProcedure;

            await con.OpenAsync();
            var result = new List<Animal>();
            using (var dr = await com.ExecuteReaderAsync())
            {
                while (await dr.ReadAsync())
                {
                    result.Add(new Animal
                    {
                        Name = dr["Name"].ToString(),
                        Description = dr["Description"].ToString()
                    });
                }
            }

            return result;
        }
        //------------------------------------------------------
        //------------------------------------------------------
        public async Task<int>  CheckIfExistsAsync(String id)
        {
            using var con = new SqlConnection(_pass.GetPassword());
            using var comSel = new SqlCommand();
            comSel.Connection = con;
            comSel.CommandText = "Select * from animal WHERE IdAnimal=@paramID;";
            comSel.Parameters.AddWithValue("@paramID", id);

            await con.OpenAsync();
            using DbTransaction tran = await con.BeginTransactionAsync();
            comSel.Transaction = (SqlTransaction)tran;
            var result = new List<Animal>();
            using (var dr = await comSel.ExecuteReaderAsync())
            {
                while (await dr.ReadAsync())
                {
                    result.Add(new Animal
                    {
                        Name = dr["Name"].ToString(),
                        Description = dr["Description"].ToString()
                    });
                }

            }
            if (result.Count == 0)
            {
                await con.CloseAsync();
                return 2;

            }
            else
            {
                await con.CloseAsync();
                return 1;
            }
                
        }
        public IEnumerable<Animal> AddAnimals(Animal am)
        {
            Animal a1 = am;


            using var con = new SqlConnection(_pass.GetPassword());
            using var com = new SqlCommand();
            com.Connection = con;
            com.CommandText = "insert into animal (Name, Description, Category, Area) VALUES(@param1,@param2,@param3, @param4);";

            com.Parameters.AddWithValue("@param1", a1.Name);
            com.Parameters.AddWithValue("@param2", a1.Description);
            com.Parameters.AddWithValue("@param3", "Category");
            com.Parameters.AddWithValue("@param4", "Area");

            con.Open();
            com.ExecuteNonQuery();

            var result = new List<Animal>();
            result.Add(new Animal
            {
                Name = a1.Name.ToString(),
                Description = a1.Description.ToString()
            });
            return result;
        }
    }

}
