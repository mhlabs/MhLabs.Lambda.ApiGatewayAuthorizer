using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.AspNetCoreServer.Internal;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MhLabs.Lambda.ApiGatewayAuthorizer
{
    public abstract class APIGatewayAuthorizerProxyFunction : APIGatewayProxyFunction
    {
        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public async Task<APIGatewayProxyResponse> FunctionHandlerAsync(APIGatewayAuthorizerProxyRequest request, ILambdaContext lambdaContext)
        {
            
            Console.WriteLine(JsonConvert.SerializeObject(request));
            if (string.IsNullOrEmpty(request.HttpMethod))
            {
                request.HttpMethod = "GET";
                request.Path = "/ping";
                request.Headers = new Dictionary<string, string> {{"Host", "localhost"}};
                request.RequestContext= new AuthorizerRequestContext();

                lambdaContext.Logger.LogLine("Keep-alive invokation");
                lambdaContext = new DummyContext(lambdaContext);
               
            }
            lambdaContext.Logger.LogLine($"Incoming {request.HttpMethod} requests to {request.Path}");
            var features = new InvokeFeatures();
            MarshallRequest(features, request);

            var context = this.CreateContext(features);

            if (request.RequestContext.Authorizer != null)
            {
                var claims = request.RequestContext.Authorizer.Claims.ToObject<Dictionary<string, object>>();
                var identity = new ClaimsIdentity(claims.Select(entry => new Claim(entry.Key, entry.Value.ToString())), "AuthorizerIdentity");
                context.HttpContext.User = new ClaimsPrincipal(identity);
            }

            // Add along the Lambda objects to the HttpContext to give access to Lambda to them in the ASP.NET Core application
            context.HttpContext.Items[LAMBDA_CONTEXT] = lambdaContext;
            context.HttpContext.Items[APIGATEWAY_REQUEST] = request;
            
            var response = await this.ProcessRequest(lambdaContext, context, features);
            
            response.IsBase64Encoded = Environment.GetEnvironmentVariable("IsBase64Encoded") != null && bool.Parse(Environment.GetEnvironmentVariable("IsBase64Encoded"));
            return response;
        }
    }

    public class DummyContext : ILambdaContext
    {
        public string AwsRequestId { get; }
        public IClientContext ClientContext { get; }
        public string FunctionName { get; }
        public string FunctionVersion { get; }
        public ICognitoIdentity Identity { get; }
        public string InvokedFunctionArn { get; }
        public ILambdaLogger Logger { get; set; }
        public string LogGroupName { get; }
        public string LogStreamName { get; }
        public int MemoryLimitInMB { get; }
        public TimeSpan RemainingTime { get; }

        public DummyContext(ILambdaContext fromContext)
        {
            AwsRequestId = fromContext.AwsRequestId;
            ClientContext = fromContext.ClientContext;
            FunctionName = fromContext.FunctionName;
            FunctionVersion = fromContext.FunctionVersion;
            Identity = fromContext.Identity;
            InvokedFunctionArn = fromContext.InvokedFunctionArn;
            LogGroupName = fromContext.LogGroupName;
            LogStreamName = fromContext.LogStreamName;
            MemoryLimitInMB = fromContext.MemoryLimitInMB;
            RemainingTime = fromContext.RemainingTime;
            Logger = new NullLogger();
        }
    }

    public class NullLogger : ILambdaLogger
    {
        public void Log(string message)
        {
            
        }

        public void LogLine(string message)
        {
            
        }
    }
}
