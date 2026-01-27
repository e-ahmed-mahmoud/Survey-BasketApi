using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyBasket.Contracts.Users;

public record RefreshTokenRequest (string Token, string RefreshToken);
