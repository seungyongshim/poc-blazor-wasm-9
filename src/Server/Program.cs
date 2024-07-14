using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.Authority = "https://dev-5ureawdsacbjt2tc.us.auth0.com";
    options.Audience = "https://dev-5ureawdsacbjt2tc.us.auth0.com/api/v2/";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidAudience = "https://dev-5ureawdsacbjt2tc.us.auth0.com/api/v2/",
        ValidIssuer = "https://dev-5ureawdsacbjt2tc.us.auth0.com"
    };


    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // 요청 헤더에서 토큰을 수신하여 로그로 출력
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            Console.WriteLine($"Received Token: {token}");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication Failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var token = context.SecurityToken as JwtSecurityToken;
            if (token != null)
            {
                Console.WriteLine($"Token Validated: {token}");
                Console.WriteLine($"Issuer: {token.Issuer}");
                Console.WriteLine($"Audience: {string.Join(", ", token.Audiences)}");
                Console.WriteLine($"Claims: {string.Join(", ", token.Claims.Select(c => $"{c.Type}: {c.Value}"))}");
            }
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", (HttpContext context) =>
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
.RequireAuthorization()
.WithName("GetWeatherForecast");

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();


app.MapFallbackToFile("index.html");


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
