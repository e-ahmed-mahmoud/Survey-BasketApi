using SurveyBasket.Abstractions.Const;
using SurveyBasket.Authentication.Authorization;
using SurveyBasket.Contracts.Roles;
using SurveyBasket.Services.RoleServices;

namespace SurveyBasket.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = DefaultRoles.Admin)]
public class RolesController(IRoleService roleService) : ControllerBase
{
    private readonly IRoleService _roleService = roleService;

    [HttpGet("[action]")]
    [HasPermission(PermissionsClaims.GetRoles)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken, [FromQuery] bool? isDeleted = false)
    {
        var result = await _roleService.GetAllAsync(isDeleted, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem(result.Error.StatusCode);
    }

    [HttpGet("[action]/{id}")]
    [HasPermission(PermissionsClaims.GetRoles)]
    public async Task<IActionResult> GetById([FromRoute] string id, CancellationToken cancellationToken)
    {
        var result = await _roleService.GetByIdAsync(id, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem(404);
    }

    [HttpPost("[action]")]
    [HasPermission(policyName: PermissionsClaims.AddRoles)]
    public async Task<IActionResult> Add([FromBody] RoleRequest roleRequest)
    {
        var result = await _roleService.AddAsync(roleRequest);

        return result.IsSuccess ? CreatedAtAction(nameof(GetById), result.Value.Id, result.Value) : result.ToProblem(result.Error.StatusCode);
    }

    [HttpPatch("[action]/{id}")]
    [HasPermission(policyName: PermissionsClaims.AddRoles)]
    public async Task<IActionResult> Update([FromRoute] string id, [FromBody] RoleRequest roleRequest)
    {
        var result = await _roleService.UpdateAsync(id, roleRequest);

        return result.IsSuccess ? NoContent() : result.ToProblem(result.Error.StatusCode);
    }


}
