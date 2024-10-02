// builder perpetually listens for requests
using DotnetAPI.Data;
using Microsoft.AspNetCore.Cors.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Angular, React, Vue
string[] singlePageApplications = new string[] {"http:\\localhost:4200", "http:\\localhost:3000", "http:\\localhost:8000"};
builder.Services.AddCors((options) =>
{
    options.AddPolicy("DevCors", (corsBuilder) =>
    {
        // List of major front end single page applications we might want to allow (Angular, React, Vue)
        // Restrict once decided which one(s) will be used
        corsBuilder.WithOrigins(singlePageApplications)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
       options.AddPolicy("ProdCors", (corsBuilder) =>
    {
        corsBuilder.WithOrigins("https:\\DomainNameOfFrontEndOnProductionSite.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();

//////////////// app below ////////////////////////

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // More for production API, redirects User to secure endpoint  
    //  and use SSL certificate
    app.UseHttpsRedirection();
}

// Setup routes
app.MapControllers();

// In our controller, we omit /weatherforecast
// because it is implied as the name of the class
// app.MapGet("/weatherforecast", () =>
// {
// .WithName("GetWeatherForecast")
// .WithOpenApi();

app.Run();
