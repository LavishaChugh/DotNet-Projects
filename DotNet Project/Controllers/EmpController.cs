using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using DotNet_Project.Model;

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

    }
}
