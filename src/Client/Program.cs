using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddHttpClient("ServerAPI", client =>
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI"));

builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority = "https://dev-5ureawdsacbjt2tc.us.auth0.com";
    options.ProviderOptions.ClientId = "p4Rubx3E4mzRwwuePXEey5n93t697g6z";
    options.ProviderOptions.AdditionalProviderParameters.Add("audience", "https://dev-5ureawdsacbjt2tc.us.auth0.com/api/v2/");
    options.ProviderOptions.ResponseType = "code";


    options.ProviderOptions.DefaultScopes.Add("openid");
    options.ProviderOptions.DefaultScopes.Add("profile");
    options.ProviderOptions.DefaultScopes.Add("email");
});

await builder.Build().RunAsync();
