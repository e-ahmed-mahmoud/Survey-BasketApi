using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace SurveyBasket.Authentication;

public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions _options = options.Value;
    public (string token, int exporesIn) GenerateJWTToken(ApplicationUser user)
    {
        //define claims, payload
        Claim[] claims = [
            new (JwtRegisteredClaimNames.Sub , user.Id),
            new (JwtRegisteredClaimNames.Email , user.Email!),
            new (JwtRegisteredClaimNames.GivenName , user.FirstName),
            new (JwtRegisteredClaimNames.FamilyName , user.LastName),
            new (JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString()),
        ];
        // define symattric security key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        // define signing credentials, signature algorithm
        SigningCredentials signingCredientials = new(key, SecurityAlgorithms.HmacSha256);

        // generate token
        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpiryInMinutes),
            signingCredentials: signingCredientials
        );

        // return token string and expires in
        return (token: new JwtSecurityTokenHandler().WriteToken(token), exporesIn: _options.ExpiryInMinutes * 60);
    }

    public string? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var semmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        try
        {

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = semmetricSecurityKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero  // remove delay of token when expire
            }, out SecurityToken validatedToken);
            var jwtToken = validatedToken as JwtSecurityToken;
            return jwtToken?.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
        }
        catch
        {
            return null;
        }

    }
}
