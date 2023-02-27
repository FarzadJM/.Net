using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthentication>("BasicAuthentication", null);
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", (HttpContext context) => context.User.Identity?.Name).RequireAuthorization();

app.Run();

public class BasicAuthentication : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public BasicAuthentication(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Context.Request.Headers.Authorization.FirstOrDefault(x => x.ToLower().StartsWith("basic")) is string authStr)
        {
            var token = authStr.Substring("basic ".Length).Trim();
            var credentialStr = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var credentials = credentialStr.Split(":");

            if (credentials.ElementAt(0) == "username" && credentials.ElementAt(1) == "password")
            {
                var claims = new[] { new Claim(ClaimTypes.Name, "farzad"), new Claim(ClaimTypes.Role, "admin") };
                var identity = new ClaimsIdentity(claims, "Basic");
                var principal = new ClaimsPrincipal(identity);
                var authenticationScheme = Scheme.Name;
                var authenticationTicket = new AuthenticationTicket(principal, authenticationScheme);
                return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
            }
        }

        Response.Headers.Add("WWW-Authenticate", "Basic");
        return Task.FromResult(AuthenticateResult.Fail("Unauthorized access"));
    }
}
