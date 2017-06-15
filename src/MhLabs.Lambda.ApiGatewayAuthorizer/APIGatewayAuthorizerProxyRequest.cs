using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json.Linq;

namespace MhLabs.Lambda.ApiGatewayAuthorizer
{
    public class APIGatewayAuthorizerProxyRequest : APIGatewayProxyRequest
    {
        public new AuthorizerRequestContext RequestContext { get; set; }
    }

    public class AuthorizerRequestContext : APIGatewayProxyRequest.ProxyRequestContext
    {
        public new Authorizer Authorizer { get; set; }
    }

    public class Authorizer
    {
        public JObject Claims { get; set; }
    }
}
