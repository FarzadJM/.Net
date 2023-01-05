using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

const string SIGN_IN_KEY = "SFHASOIHFASJGOIASHEF";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer((configureOptions) =>
{
    configureOptions.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SIGN_IN_KEY))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/SignIn", ([FromBody] Login login) =>
{
    if (login.Username.ToLower() == "John".ToLower())
    {
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SIGN_IN_KEY));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, login.Username), new Claim(ClaimTypes.Role, "Admin") };
        var jwtSecurityToken = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddSeconds(30), signingCredentials: signingCredentials);

        return Results.Ok(new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken));
    }
    else
    {
        return Results.NotFound("User not found!");
    }
});

app.MapGet("/", [Authorize(Roles = "Admin")] (HttpContext httpContext) => Results.Ok((httpContext.User.Identity as ClaimsIdentity)?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value));

app.Run();

record Login(string Username);