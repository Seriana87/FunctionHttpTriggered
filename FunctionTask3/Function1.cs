using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Configuration;

namespace ServerLessExample
{
    public class Function1
    {
        private readonly IConfiguration _configuration;

        public Function1(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [FunctionName("ServerlessExample")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var azureBlobStorage = new AzureBlobStorageHelper(_configuration);

            if (req.ContentLength != null)
            {
                var id = Guid.NewGuid();
                long? body = req.ContentLength;

                Event eventTosend = FormatEventToMessage(id, body);

                string message = JsonConvert.SerializeObject(eventTosend);

                // Send out to Blob
                var filePath = DateTime.UtcNow.Date.ToString("yyyy") + "/" + DateTime.UtcNow.Date.ToString("MM") + "/" + DateTime.UtcNow.Date.ToString("dd") + "/" + id;
                CloudBlobContainer response = await azureBlobStorage.AddAsync(filePath, message);
                string responseMessage = $"The file with {id}, is saved with the path {response.Uri.AbsoluteUri}" + "/ " + filePath;
                return new OkObjectResult(responseMessage);
            }
            else
                return new OkObjectResult("This HTTP triggered function executed successfully.Pass a name in the query string or in the request body for a personalized response.");
        }

        private static Event FormatEventToMessage(Guid id, long? body)
        {
            return new Event
            {
                Header = new Header
                {
                    Id = id
                },
                Body = new Body
                {
                    Length = body
                }
            };
        }
    }
}
