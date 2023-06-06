using Blazored.Modal;

using DioRed.Auth.Client;
using DioRed.Murka.Core;
using DioRed.Murka.Manager;
using DioRed.Murka.Manager.Data;

using Grpc.Net.Client;
using Grpc.Net.Client.Web;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton(services =>
{
    var httpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());

    return GrpcChannel.ForAddress(builder.Configuration["serviceUrl"]!, new GrpcChannelOptions { HttpHandler = httpHandler });
});

var authConfiguration = AuthClientConfiguration.Load(builder.Configuration.GetSection("auth"));
var authClient = new AuthClient(authConfiguration);

builder.Services.AddSingleton(new ApiSettings
{
    Uri = builder.Configuration["apiUri"]!,
    AccessToken = authClient.GetAccessToken()
});

builder.Services.AddSingleton<ApiClient>();
builder.Services.AddSingleton<ILogic, Logic>();

builder.Services.AddBlazoredModal();

builder.Services.AddSingleton<DataProvider>();

await builder.Build().RunAsync();
