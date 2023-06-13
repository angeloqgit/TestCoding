using GithubUsers.Api.Services;
using GithubUsers.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
//builder.Services.AddOptions();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.Configure<AppSettingsModel>(builder.Configuration.GetSection("ApiSettings")); 
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
