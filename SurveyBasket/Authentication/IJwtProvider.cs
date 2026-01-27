using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyBasket.Authentication;

public interface IJwtProvider
{
    (string token , int exporesIn ) GenerateJWTToken( ApplicationUser user); 
    string? ValidateToken(string token);
}
