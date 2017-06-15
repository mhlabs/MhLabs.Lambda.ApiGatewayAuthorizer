using Amazon.Lambda.TestUtilities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using TestWebApp;
using Xunit;

namespace MhLabs.Lambda.ApiGatewayAuthorizer.Test
{
    public class ValuesControllerTests
    {
        [Fact]
        public async Task TestGetWithAuthorization()
        {
            var lambdaFunction = new LambdaEntryPoint();

            var requestStr = File.ReadAllText(Directory.GetCurrentDirectory() + "/Requests/ValuesController-Get.json");
            var request = JsonConvert.DeserializeObject<APIGatewayAuthorizerProxyRequest>(requestStr);
            var context = new TestLambdaContext();
            var response = await lambdaFunction.FunctionHandlerAsync(request, context);

            Assert.Equal(response.StatusCode, 200);
            Assert.Equal("[\"value1\",\"value2\"]", response.Body);
            Assert.True(response.Headers.ContainsKey("Content-Type"));
            Assert.Equal("application/json; charset=utf-8", response.Headers["Content-Type"]);
        }

        [Fact]
        public async Task TestGetWithoutAuthorization()
        {
            var lambdaFunction = new LambdaEntryPoint();

            var requestStr = File.ReadAllText(Directory.GetCurrentDirectory() + "/Requests/ValuesController-Get.json");
            var request = JsonConvert.DeserializeObject<APIGatewayAuthorizerProxyRequest>(requestStr);

            request.RequestContext.Authorizer = null;
            var context = new TestLambdaContext();
            var response = await lambdaFunction.FunctionHandlerAsync(request, context);

            Assert.Equal(response.StatusCode, StatusCodes.Status401Unauthorized);
        }

    }
}
