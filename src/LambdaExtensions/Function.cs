using System.Net.Http.Headers;
using System.Net.Http.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Microsoft.AspNetCore.WebUtilities;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaExtensions;

public record Parameter(string Name, string Value, int Version);
public record GetParameterResponse(Parameter Parameter);
public record GetSecretValueResponse(string Name, string SecretString);
public record PartnerFee(string Name, double Percent);

public class Function
{

    public Function() { }

    public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
    {
        var port = Environment.GetEnvironmentVariable("PARAMETERS_SECRETS_EXTENSION_HTTP_PORT");
        var sessionToken = Environment.GetEnvironmentVariable("AWS_SESSION_TOKEN");

        using var httpClientLayer = new HttpClient();
        httpClientLayer.BaseAddress = new Uri($"http://localhost:{port}");
        httpClientLayer.DefaultRequestHeaders.Add("X-Aws-Parameters-Secrets-Token", sessionToken);

        var pathSecret = BuildPath(path: "/secretsmanager/get",
                                key: "secretId",
                                value: "/Partner/ServiceToken");

        var pathParameter = BuildPath(path: "/systemsmanager/parameters/get",
                                    key: "name",
                                    value: "/Partner/Host");

        var serviceTokenSecret = await httpClientLayer.GetFromJsonAsync<GetSecretValueResponse>(pathSecret);
        var partnerHostParameter = await httpClientLayer.GetFromJsonAsync<GetParameterResponse>(pathParameter);

        using var httpClientPartner = new HttpClient();
        httpClientPartner.BaseAddress = new Uri(partnerHostParameter.Parameter.Value);
        httpClientPartner.DefaultRequestHeaders
                         .Authorization = new AuthenticationHeaderValue(scheme: "Bearer",
                                                    parameter: serviceTokenSecret?.SecretString);
        foreach (var message in evnt.Records)
            await ProcessMessageAsync(httpClientPartner, message, context);

    }

    private static string BuildPath(string path, string key, string value)
    {
        var query = new Dictionary<string, string>(1)
        {
            [key] = value
        };
        return QueryHelpers.AddQueryString(path, query);
    }

    private async Task ProcessMessageAsync(HttpClient httpClientPartner, SQSEvent.SQSMessage message, ILambdaContext context)
    {
        var fee = await httpClientPartner.GetFromJsonAsync<PartnerFee>($"/fees/{message.Body}");
        context.Logger.LogInformation($"Fee {fee.Name}: {fee.Percent:P}");
    }

}