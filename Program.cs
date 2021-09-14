using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Test
{
    public class Program
    {
        public static async Task Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IConfiguration>((serviceProvider) => BuildConfigurationService(serviceProvider));
                })
                .Build();

            await host.RunAsync();
        }

        public static IConfiguration BuildConfigurationService(IServiceProvider serviceProvider)
        {
            var executionContextOptions = serviceProvider.GetService<IOptions<ExecutionContextOptions>>().Value;

            var configuration = new ConfigurationBuilder()
               .Build();

            return configuration;
        }
    }

    public class KustoMonitorFunctionApp
    {
        private readonly IConfiguration _config;

        public KustoMonitorFunctionApp(IConfiguration config)
        {
            _config = config;
        }

        [Function("Echo")]
        public async Task<HttpResponseData> EchoAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")]
            HttpRequestData req,
            FunctionContext _)
        {
            var body = await req.ReadAsStringAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString($"ECHO: {body}");

            return response;
        }
    }
}