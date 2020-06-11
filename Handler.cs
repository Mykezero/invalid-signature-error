using Amazon.Lambda.Core;
using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Lambda;
using Amazon.Lambda.Model;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AwsDotnetCsharp
{
    public class Handler
    {
        public async Task<string> MainLambda()
        {
            string response = await StartWorkerLambda();
            return await Task.FromResult("Complete!");
        }

        private async Task<string> StartWorkerLambda()
        {
            using AmazonLambdaClient client = new AmazonLambdaClient();
            InvokeResponse invokeResponse = await client.InvokeAsync(new InvokeRequest()
            {
                FunctionName = "invalidsignature-dev-WorkerLambda"
            });
            using StreamReader reader = new StreamReader(invokeResponse.Payload);
            return await reader.ReadToEndAsync();
        }

        public async Task<string> WorkerLambda()
        {
            // Exceed 6 MB response limit
            int megabytes = 7;
            // 7 MB: 7340032
            int bytesPerMegabyte = (int)Math.Pow(1024, 2) * megabytes;
            // 1 byte
            char oneByteCharacter = 'x';
            // 7 MB string
            string result = new string(oneByteCharacter, bytesPerMegabyte);
            // Exceed signature valid (5 min.)
            await Task.Delay(TimeSpan.FromMinutes(6));
            // Send response exceeding response limit (> 6MB)
            return result;
        }
    }
}