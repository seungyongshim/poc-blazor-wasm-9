// https://auth0.com/blog/backend-for-frontend-pattern-with-auth0-and-dotnet/
// https://github.com/DuendeSoftware/Samples/tree/main/IdentityServer/v5/BFF/BlazorWasm

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
