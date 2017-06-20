using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.AspNetCoreServer.Internal;
using Amazon.Lambda.Core;

namespace MhLabs.Lambda.ApiGatewayAuthorizer
{
    public abstract class APIGatewayAuthorizerProxyFunction : APIGatewayProxyFunction
    {
        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public async Task<APIGatewayProxyResponse> FunctionHandlerAsync(APIGatewayAuthorizerProxyRequest request, ILambdaContext lambdaContext)
        {
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

            return response;
        }
    }
}
