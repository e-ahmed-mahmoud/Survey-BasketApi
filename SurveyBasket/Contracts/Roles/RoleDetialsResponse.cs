using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyBasket.Contracts.Roles;

public record RoleDetialsResponse(string Id, string Name, bool IsDeleted, IEnumerable<string> RoleClaims);

