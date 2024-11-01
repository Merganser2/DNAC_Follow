// builder perpetually listens for requests
using System.Text;
using DotnetAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.IdentityModel.Tokens;

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

// Ensure token key is the same as in AuthController - get Configuration from builder
string? tokenKeyString = builder.Configuration.GetSection("AppSettings:TokenKey").Value;
SymmetricSecurityKey symmetricTokenKey = new(Encoding.UTF8.GetBytes(tokenKeyString ?? ""));

// Making it flexible so we can use PostMan, create new token with Jwt.io, 
TokenValidationParameters tokenValidationParameters = new TokenValidationParameters(){
    IssuerSigningKey = symmetricTokenKey,
    ValidateIssuer = false,
    ValidateIssuerSigningKey = false,
    ValidateAudience = false
};

// "Bearer <token>", common way to do this
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = tokenValidationParameters;
                });

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

app.UseAuthentication();

app.UseAuthorization();

// Setup routes
app.MapControllers();

// In our controller, we omit /weatherforecast
// because it is implied as the name of the class
// app.MapGet("/weatherforecast", () =>
// {
// .WithName("GetWeatherForecast")
// .WithOpenApi();

app.Run();
