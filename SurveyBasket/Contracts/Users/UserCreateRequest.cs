using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyBasket.Contracts.Users;

public record UserCreateRequest(string FirstName, string LastName, string Password, string PhoneNumber, string Email, IList<string> Roles);
