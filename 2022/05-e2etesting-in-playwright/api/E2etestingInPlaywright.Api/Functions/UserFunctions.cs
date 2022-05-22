using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using E2etestingInPlaywright.Api.Repositories;
using E2etestingInPlaywright.Api.Models;

namespace E2etestingInPlaywright.Api.Functions
{
    public class UserFunctions
    {
        private readonly IUserData _data;
        public UserFunctions(
            IUserData data
        )
        {
            _data = data;
        }

        [FunctionName("GetUsers")]
        public async Task<IActionResult> GetUsers(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "user")] HttpRequest req,
            ILogger log
        )
        {
            var datas = await _data.GetAllDatasAsync();
            // await Task.Delay(10000);
            return new OkObjectResult(datas);
        }

        [FunctionName("GetUser")]
        public async Task<IActionResult> GetUser(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "user/{id}")] HttpRequest req,
            int id,
            ILogger log
        )
        {
            var data = await _data.GetDataAsync(id);
            if (data == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(data);
        }

        [FunctionName("PostUser")]
        public async Task<IActionResult> PostUser(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "user")] HttpRequest req,
            ILogger log
        )
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = Utf8Json.JsonSerializer.Deserialize<User>(requestBody);
            await _data.UpsertAsync(data);
            return new OkResult();
        }

        [FunctionName("DeleteUser")]
        public async Task<IActionResult> DeleteUser(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "user/{id}")] HttpRequest req,
            int id,
            ILogger log
        )
        {
            await _data.DeletetAsync(id);
            return new OkResult();
        }
    }
}
