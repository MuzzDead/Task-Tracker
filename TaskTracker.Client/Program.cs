using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Refit;
using TaskTracker.Client;
using TaskTracker.Client.Handlers;
using TaskTracker.Client.Services;
using TaskTracker.Client.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAntDesign();

builder.Services.AddBlazoredLocalStorage();

var apiBaseUrl = builder.Configuration["ApiBaseUrl"];

builder.Services.AddScoped<IAuthStateService, AuthStateService>();
builder.Services.AddScoped<AuthenticationHandler>();

builder.Services
    .AddRefitClient<IBoardService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<AuthenticationHandler>();

builder.Services
    .AddRefitClient<ICardService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<AuthenticationHandler>();

builder.Services
    .AddRefitClient<IColumnService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<AuthenticationHandler>();

builder.Services
    .AddRefitClient<IAuthService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));

builder.Services
    .AddRefitClient<ICommentService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<AuthenticationHandler>();

builder.Services
    .AddRefitClient<IUserService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<AuthenticationHandler>();

builder.Services
    .AddRefitClient<IBoardRoleService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<AuthenticationHandler>();

builder.Services
    .AddRefitClient<IStateService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<AuthenticationHandler>();


builder.Services.AddScoped<IBoardPageService, BoardPageService>();
builder.Services.AddScoped<ICardModalService, CardModalService>();
builder.Services.AddScoped<IPasswordHashingService, PasswordHashingService>();

await builder.Build().RunAsync();
