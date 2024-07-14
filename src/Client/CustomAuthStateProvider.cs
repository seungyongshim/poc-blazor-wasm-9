using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Security.Claims;

public class AccessTokenProvider : IAccessTokenProvider
{
    public ValueTask<AccessTokenResult> RequestAccessToken() => throw new NotImplementedException();
    public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options) => throw new NotImplementedException();
}
