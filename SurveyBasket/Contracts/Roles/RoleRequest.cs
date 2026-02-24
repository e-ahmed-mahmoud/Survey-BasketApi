using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyBasket.Contracts.Roles;

public record RoleRequest(string Name, IList<string> Permissions);
