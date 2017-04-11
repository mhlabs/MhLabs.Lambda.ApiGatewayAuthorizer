using Newtonsoft.Json.Linq;

namespace MhLabs.Lambda.ApiGatewayAuthorizer
{
    public class APIGatewayAuthorizerProxyRequest
    {
        public AuthorizerRequestContext RequestContext { get; set; }
        public class AuthorizerRequestContext
        {
            public JObject Authorizer { get; set; }
        }

    }
}
