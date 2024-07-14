using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var auth0Domain = "https://dev-5ureawdsacbjt2tc.us.auth0.com";
var auth0Audience = "https://dev-5ureawdsacbjt2tc.us.auth0.com/api/v2/";
var auth0ClientId = "p4Rubx3E4mzRwwuePXEey5n93t697g6z";


builder.Services.AddOpenApi();
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.Authority = auth0Domain;
    options.Audience = auth0Audience;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name"
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
    app.UseWebAssemblyDebugging();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/api/auth/login", (HttpContext context) =>
{
    var redirectUri = "https://localhost:7099/api/auth/callback"; // BFF API의 콜백 엔드포인트
    var state = Guid.NewGuid().ToString("N");

    var loginUrl = $"{auth0Domain}/authorize?response_type=code&client_id={auth0ClientId}&redirect_uri={redirectUri}&state={state}&scope=openid profile email";

    return Results.Ok(loginUrl);
});

app.MapGet("/api/auth/callback", async (HttpContext context) =>
{
    var code = context.Request.Query["code"];
    var redirectUri = "https://localhost:7099/api/auth/callback";

    using var client = new HttpClient();
    var tokenResponse = await client.PostAsync($"{auth0Domain}/oauth/token", new FormUrlEncodedContent(
    [
        new KeyValuePair<string, string>("grant_type", "authorization_code"),
        new KeyValuePair<string, string>("client_id", auth0ClientId),
        new KeyValuePair<string, string>("code", "code"),
        new KeyValuePair<string, string>("redirect_uri", redirectUri),
    ]));

    var tokenContent = await tokenResponse.Content.ReadFromJsonAsync<Auth0TokenResponse>();
    // 토큰을 저장하거나 필요한 처리를 합니다.
    context.Response.Cookies.Append("AuthToken", tokenContent?.IdToken ?? "");

    context.Response.Redirect("/");
    return Results.Ok();
});

app.MapGet("/api/auth/userinfo", async (HttpContext context) =>
{
    var token = context.Request.Cookies["AuthToken"];
    if (string.IsNullOrEmpty(token))
    {
        return Results.Ok(new { IsAuthenticated = false });
    }

    using var client = new HttpClient();
    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    var userInfoResponse = await client.GetFromJsonAsync<UserInfoResponse>($"{auth0Domain}/userinfo");

    return Results.Ok(new { IsAuthenticated = true, Name = userInfoResponse?.Name ?? ""});
});

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

public record Auth0TokenResponse(string IdToken);
public record UserInfoResponse(string Name);
