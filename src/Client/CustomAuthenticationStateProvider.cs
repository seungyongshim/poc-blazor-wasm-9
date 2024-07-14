using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;

public class CustomAuthenticationStateProvider(HttpClient httpClient): AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        UserInfo? userInfo;
        try
        {
            userInfo = await httpClient.GetFromJsonAsync<UserInfo>("api/auth/userinfo");
        }
        catch(Exception ex) 
        {
            userInfo = new UserInfo { IsAuthenticated = false };
        }

        var identity = userInfo?.IsAuthenticated ?? false
        ? new ClaimsIdentity(
        [
            new(ClaimTypes.Name, userInfo?.Name ?? "")
        ], "auth0")
        : new ClaimsIdentity();

        var user = new ClaimsPrincipal(identity);
        return new AuthenticationState(user);
    }

    public void NotifyUserAuthentication(ClaimsPrincipal user)
    {
        var authState = Task.FromResult(new AuthenticationState(user));
        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        NotifyAuthenticationStateChanged(authState);
    }
}

public class UserInfo
{
    public bool IsAuthenticated { get; set; }
    public string? Name { get; set; }
}
