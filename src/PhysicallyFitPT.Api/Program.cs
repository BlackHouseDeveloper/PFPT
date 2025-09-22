#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1633 // The file name must match the first type name
#pragma warning disable SA1009 // Closing parenthesis should be spaced correctly
#pragma warning disable SA1025 // Code should not contain multiple whitespace in a row
#pragma warning disable SA1507 // Code should not contain multiple blank lines in a row
#pragma warning disable SA1028 // Code should not contain trailing whitespace
#pragma warning disable SA1649 // File name should match first type name
#pragma warning disable SA1413 // Use trailing comma in multi-line initializers
#pragma warning disable SA1101 // Prefix local calls with 'this.'
#pragma warning disable SA1110 // Opening parenthesis should be on the line of the previous token
#pragma warning disable SA1111 // Closing parenthesis should be on its own line
#pragma warning disable SA1400 // Access modifier should be declared


var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

#pragma warning restore SA1400 // Access modifier should be declared
#pragma warning restore SA1111 // Closing parenthesis should be on its own line
#pragma warning restore SA1110 // Opening parenthesis should be on the line of the previous token
#pragma warning restore SA1101 // Prefix local calls with 'this.'
#pragma warning restore SA1413 // Use trailing comma in multi-line initializers
#pragma warning restore SA1649 // File name should match first type name    
#pragma warning restore SA1028 // Code should not contain trailing whitespace
#pragma warning restore SA1507 // Code should not contain multiple blank lines in a row
#pragma warning restore SA1025 // Code should not contain multiple whitespace in a row
#pragma warning restore SA1009 // Closing parenthesis should be spaced correctly
#pragma warning restore SA1633 // The file name must match the first type name
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore SA1600 // Elements should be documented
