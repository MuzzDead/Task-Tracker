using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using TaskTracker.Application.Extensions;
using TaskTracker.Database;
using TaskTracker.Infrastructure.Extensions;
using TaskTracker.Persistence.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowBlazorClient",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:5064",
                    "https://localhost:5064"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

// Add services to the container.
builder.Services.AddControllers();

// Application-layer services
builder.Services.AddApplicationServices();

// Infrastructure-layer services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Persistence-layer services
builder.Services.AddPersistenceServices(builder.Configuration);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskTracker API", Version = "v1" });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Description = "Enter JWT Bearer token only",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    opt.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
DatabaseMigrator.MigrateDatabase(connectionString);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazorClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
