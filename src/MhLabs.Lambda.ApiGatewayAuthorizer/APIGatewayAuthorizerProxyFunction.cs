using System.Collections.Generic;
using System.IO;
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
        private ILambdaSerializer _serializer = (ILambdaSerializer)new Amazon.Lambda.Serialization.Json.JsonSerializer();

        public override async Task<Stream> FunctionHandlerAsync(Stream requestStream, ILambdaContext lambdaContext)
        {
            if (EnableRequestLogging)
            {
                lambdaContext.Logger.LogLine(new StreamReader(requestStream).ReadToEnd());
                requestStream.Position = 0L;
            }
            var claimsRequest = this._serializer.Deserialize<APIGatewayAuthorizerProxyRequest>(requestStream);
            requestStream.Position = 0L;
            var apiGatewayRequest = this._serializer.Deserialize<APIGatewayProxyRequest>(requestStream);
            lambdaContext.Logger.Log(string.Format("Incoming {0} requests to {1}", (object)apiGatewayRequest.HttpMethod, (object)apiGatewayRequest.Path));
            InvokeFeatures features = new InvokeFeatures();
            this.MarshallRequest(features, apiGatewayRequest);
            var context = CreateContext(features);

            if (claimsRequest.RequestContext.Authorizer != null)
            {
                var claims = claimsRequest.RequestContext.Authorizer.ToObject<Dictionary<string, object>>();
                var identity = new ClaimsIdentity(claims.Select(entry => new Claim(entry.Key, entry.Value.ToString())));
                context.HttpContext.User.AddIdentity(identity);
            }

            APIGatewayProxyResponse response = await ProcessRequest(lambdaContext, context, features, false);
            MemoryStream memoryStream = new MemoryStream();
            _serializer.Serialize(response, memoryStream);
            memoryStream.Position = 0L;
            if (EnableResponseLogging)
            {
                lambdaContext.Logger.LogLine(new StreamReader(memoryStream).ReadToEnd());
                memoryStream.Position = 0L;
            }
            return memoryStream;
        }        
    }
}
