using Grpc.Net.Client;
using Grpc.Net.Client.Web;

using DioRed.Murka.Manager;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.Modal;
using DioRed.Murka.Manager.Data;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton(services =>
{
    var httpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());

    return GrpcChannel.ForAddress(builder.Configuration["serviceUrl"]!, new GrpcChannelOptions { HttpHandler = httpHandler });
});

builder.Services.AddBlazoredModal();

builder.Services.AddSingleton<DataProvider>();

await builder.Build().RunAsync();
