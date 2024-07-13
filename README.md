![CI](../../workflows/CI/badge.svg) ![Cov](../gh-pages/docs/badge_linecoverage.svg)

## dotnet 7의 경우
`dotnet new blazorwasm --hosted`

## dotnet 9의 경우
```
// client
dotnet new blazorwasm

// server
dotnet new webapi --minimalapi
dotnet add package Microsoft.AspNetCore.Components.WebAssembly.Server --prerelease

// program.cs
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");
```
