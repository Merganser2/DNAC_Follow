// builder perpetually listens for requests
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else {
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
