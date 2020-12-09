using System;
using System.Threading.Tasks;

using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.Rest;
using Newtonsoft.Json.Linq;

namespace MyFunctions {

    public static class MyBlobFunction {

        private static string STORAGE_CONNECTION_STRING;

        static MyBlobFunction() {
            STORAGE_CONNECTION_STRING = System.Environment.GetEnvironmentVariable("FileStorage", EnvironmentVariableTarget.Process);
        }

        [FunctionName("MyFileFunction")]
        public static async Task Run(
            [EventGridTrigger]EventGridEvent eventGridEvent,
            ILogger log
        ) {

            log.LogInformation($"Event Grid Info: {eventGridEvent.Data}");

            var data = ((JObject)eventGridEvent.Data).ToObject<StorageBlobCreatedEventData>();
            var fileUri = new Uri(data.Url);

            var dummyClient = new BlobClient(fileUri);
            var blobClient = new BlobClient(STORAGE_CONNECTION_STRING, dummyClient.BlobContainerName, dummyClient.Name);
            await blobClient.SetAccessTierAsync(AccessTier.Cool);
        }
    }
}