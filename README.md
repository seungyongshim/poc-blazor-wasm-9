![CI](../../workflows/CI/badge.svg) ![Cov](../gh-pages/docs/badge_linecoverage.svg)

## dotnet 7의 경우
`dotnet new blazorwasm --hosted`

## dotnet 9의 경우
```
// client
dotnet new blazorwasm
dotnet add package Microsoft.AspNetCore.Components.WebAssembly.Authentication --prerelease

// server
dotnet new webapi -minimal
dotnet add package Microsoft.AspNetCore.Components.WebAssembly.Server --prerelease


// program.cs
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");
```


## 인증
https://auth0.com/blog/securing-blazor-webassembly-apps/#Adding-Support-for-Authentication

## 로그인 폼 
https://www.prowaretech.com/articles/current/blazor/wasm/login-form#!


## 해볼만한 샘플
* https://github.com/dotnet/blazor-samples/tree/main/8.0/BlazorWebAppOidcBff