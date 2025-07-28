using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Refit;
using TaskTracker.Client;
using TaskTracker.Client.Services;
using TaskTracker.Client.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAntDesign();

builder.Services.AddBlazoredLocalStorage();

var apiBaseUrl = builder.Configuration["ApiBaseUrl"];

builder.Services
    .AddRefitClient<IBoardService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));

builder.Services
    .AddRefitClient<ICardService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));

builder.Services
    .AddRefitClient<IColumnService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));

builder.Services
    .AddRefitClient<IAuthService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));

builder.Services
    .AddRefitClient<ICommentService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));


builder.Services.AddScoped<IAuthStateService, AuthStateService>();
builder.Services.AddScoped<IPasswordHashingService, PasswordHashingService>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

await builder.Build().RunAsync();
