using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TaskTracker.Client;
using TaskTracker.Client.Services;
using TaskTracker.Client.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAntDesign();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri("https://localhost:7045/") }
);

builder.Services.AddScoped<IBoardService, BoardService>();

await builder.Build().RunAsync();
