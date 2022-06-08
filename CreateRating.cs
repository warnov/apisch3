using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Company.Function
{
    public static class CreateRating
    {
        [FunctionName("CreateRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Rating data = JsonConvert.DeserializeObject<Rating>(requestBody);

            if (!data.Validate())
                return new BadRequestObjectResult("Invalid Rating");

            data.CompleteInfo();
            data.SaveRating();

            return new OkObjectResult(data);
        }
    }
}
