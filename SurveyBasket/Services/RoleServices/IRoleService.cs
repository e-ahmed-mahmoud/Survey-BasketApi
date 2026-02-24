using SurveyBasket.Contracts.Roles;

namespace SurveyBasket.Services.RoleServices;

public interface IRoleService
{
    Task<Result<RoleDetialsResponse>> AddAsync(RoleRequest roleRequest);
    Task<Result<IEnumerable<RolesResponse>>> GetAllAsync(bool? isDeleted, CancellationToken cancellationToken = default);

    Task<Result<RoleDetialsResponse>> GetByIdAsync(string Id, CancellationToken cancellationToken = default);
    Task<Result<RoleDetialsResponse>> UpdateAsync(string id, RoleRequest roleRequest, CancellationToken cancellationToken = default);
}