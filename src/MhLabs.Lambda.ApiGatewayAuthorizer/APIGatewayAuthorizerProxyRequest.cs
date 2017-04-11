using Newtonsoft.Json.Linq;

namespace MhLabs.Lambda.ApiGatewayAuthorizer
{
    public class APIGatewayAuthorizerProxyRequest
    {
        public AuthorizerRequestContext RequestContext { get; set; }
        public class AuthorizerRequestContext
        {
            public Authorizer Authorizer { get; set; }
        }

        public class Authorizer
        {
            public JObject Claims { get; set; }
        }
    }

}
