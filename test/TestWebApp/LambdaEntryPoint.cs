using MhLabs.Lambda.ApiGatewayAuthorizer;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace TestWebApp
{
    /// <summary>
    /// This class extends from APIGatewayProxyFunction which contains the method FunctionHandlerAsync which is the 
    /// actual Lambda function entry point. The Lambda handler field should be set to
    /// 
    /// MhLabs.Lambda.ApiGatewayAuthorizer.Test::MhLabs.Lambda.ApiGatewayAuthorizer.Test.LambdaEntryPoint::FunctionHandlerAsync
    /// </summary>
    public class LambdaEntryPoint : APIGatewayAuthorizerProxyFunction
    {
        protected override void Init(IWebHostBuilder builder)
        {
            builder
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseApiGateway();
        }
    }
}
