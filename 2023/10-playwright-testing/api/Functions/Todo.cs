using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PlaywrightTesting.Api.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using System.Collections;
using System.Linq;

namespace PlaywrightTesting.Api.Functions
{
    public class Todo
    {

        private readonly string _connectionString = "";

        public Todo()
        {
            _connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
        }

        [FunctionName("TodoGet")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            using (var connection = new SqlConnection(_connectionString))
            {
                var res = await connection.QueryAsync<TodoModel>("GetTodos", commandType: CommandType.StoredProcedure);
                return new OkObjectResult(res);
            }
        }

        [FunctionName("TodoPost")]
        public async Task<IActionResult> Post(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequest req,
            ILogger log
        )
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string description = data?.description;

            using (var connection = new SqlConnection(_connectionString))
            {
                var param = new { Description = description };
                var res = await connection.QueryAsync<TodoModel>("AddTodo", param, commandType: CommandType.StoredProcedure);
                if (res.Any())
                {
                    return new OkObjectResult(res.First());
                }
            }
            return new EmptyResult();
        }

        [FunctionName("TodoPut")]
        public async Task<IActionResult> Put(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            int id = data?.id;

            using (var connection = new SqlConnection(_connectionString))
            {
                var param = new { Id = id };
                await connection.ExecuteAsync("UpdateStatus", param, commandType: CommandType.StoredProcedure);
                return new OkResult();
            }
        }
    }
}
