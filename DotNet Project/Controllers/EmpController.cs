using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using DotNet_Project.Model;
using System.Data;

namespace DotNet_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public EmpController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        //GetAll
        [HttpGet]
        public async Task<ActionResult<List<Emp>>> GetAll()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var employees = await connection.QueryAsync<Emp>("Select * from Emp");    //SELECT query

            return Ok(employees);
        }


        //Get by ID
        [HttpGet("{id}")]

        public async Task<ActionResult<Emp>> Get(int id)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var employee = await connection.QueryAsync<Emp>($"Select * from Emp where id ={id}");

            return Ok(employee);
        }

        //DELETE
        [HttpDelete("{id}")]

        public async Task<ActionResult<List<Emp>>> DeleteEmp(int id)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            await connection.ExecuteAsync($"Delete from Emp where id ={id}");   //DELETE query

            var employees = await connection.QueryAsync("Select * from Emp");

            return Ok(employees);

        }

        //POST
        [HttpPost]

        public async Task<ActionResult<List<Emp>>> AddEmp(Emp emp)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            await connection.ExecuteAsync("Insert into Emp values (@id, @name, @email, @phone)", emp);       //INSERT

            var employees = await connection.QueryAsync("Select * from Emp");

            return Ok(employees);

        }

        //PUT
        [HttpPut]
        public async Task<ActionResult<List<Emp>>> UpdateEmp(Emp emp)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            await connection.ExecuteAsync($"update Emp set name=@name, email=@email, phone=@phone where id=@id", emp);   //UPDATE query

            var employees = await connection.QueryAsync("Select * from Emp");

            return Ok(employees);

        }


        //GetAll With STORED PROCEDURES

        [HttpGet("Stored-procedure/Get-All")]
        public async Task<ActionResult<List<Emp>>> GetAllSP()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            List<Emp> employees = new List<Emp>();

            await connection.OpenAsync();

            using (SqlCommand command = new SqlCommand("GetAll", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {

                        Emp emp = new Emp
                        {

                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Phone = reader.GetInt32(reader.GetOrdinal("phone")),


                        };

                        employees.Add(emp);
                    }
                }
            }

            connection.Close();

            return Ok(employees);
        }



        //POST with stored procedure

        [HttpPost("Stored-procedure/AddEmp")]
        public async Task<ActionResult<List<Emp>>> AddEmpSP(Emp employee)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            await connection.OpenAsync();

            using (SqlCommand insertCommand = new SqlCommand("InsertData", connection))
            {
                insertCommand.CommandType = CommandType.StoredProcedure;


                insertCommand.Parameters.Add("@ID", SqlDbType.Int).Value = employee.Id;
                insertCommand.Parameters.Add("@Name", SqlDbType.VarChar).Value = employee.Name;
                insertCommand.Parameters.Add("@Email", SqlDbType.VarChar).Value = employee.Email;
                insertCommand.Parameters.Add("@Phone", SqlDbType.Int).Value = employee.Phone;

                await insertCommand.ExecuteNonQueryAsync();
            }

            List<Emp> employees = new List<Emp>();

            using (SqlCommand command = new SqlCommand("GetAll", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Emp emp = new Emp
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Phone = reader.GetInt32(reader.GetOrdinal("phone")),
                        };

                        employees.Add(emp);
                    }
                }
            }

            connection.Close();

            return Ok(employees);
        }


        //UPDATE with stored procedure

        [HttpPut("Stored-procedure/UpdateEmp")]
        public async Task<ActionResult<List<Emp>>> UpdateEmpSP(Emp employee)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            await connection.OpenAsync();


            using (SqlCommand command = new SqlCommand("UpdateEmp", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@Id", SqlDbType.Int).Value = employee.Id;
                command.Parameters.Add("@Name", SqlDbType.VarChar).Value = employee.Name;
                command.Parameters.Add("@Email", SqlDbType.VarChar).Value = employee.Email;
                command.Parameters.Add("@Phone", SqlDbType.VarChar).Value = employee.Phone;

                await command.ExecuteNonQueryAsync();
            }

            List<Emp> employees = new List<Emp>();

            using (SqlCommand selectCommand = new SqlCommand("GetAll", connection))
            {
                selectCommand.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader reader = await selectCommand.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {

                        Emp emp = new Emp
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Phone = reader.GetInt32(reader.GetOrdinal("phone")),
                        };

                        employees.Add(emp);
                    }
                }
            }

            connection.Close();

            return Ok(employees);
        }


        //GET by ID with stored procedure

        [HttpGet("Stored-procedure/GetEmp/{id}")]
        public async Task<ActionResult<Emp>> GetEmpByIdSP(int id)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            await connection.OpenAsync();

            Emp employee = null;


            using (SqlCommand command = new SqlCommand("GetById", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        employee = new Emp
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Phone = reader.GetInt32(reader.GetOrdinal("phone")),
                        };
                    }
                }
            }

            connection.Close();

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }




        //DELETE with stored-procedure

        [HttpDelete("Stored-procedure/DeleteEmp/{id}")]
        public async Task<ActionResult<List<Emp>>> DeleteEmpSP(int id)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            await connection.OpenAsync();

            using (SqlCommand command = new SqlCommand("DeleteEmp", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                await command.ExecuteNonQueryAsync();     //do not return data
            }

            List<Emp> employees = new List<Emp>();

            using (SqlCommand selectCommand = new SqlCommand("GetAll", connection))
            {
                selectCommand.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader reader = await selectCommand.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Emp emp = new Emp
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Phone = reader.GetInt32(reader.GetOrdinal("phone")),
                        };

                        employees.Add(emp);
                    }
                }
            }

            connection.Close();

            return Ok(employees);
        }



    }

}
