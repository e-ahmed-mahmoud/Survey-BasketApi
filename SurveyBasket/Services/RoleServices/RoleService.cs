using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyBasket.Abstractions.Const;
using SurveyBasket.Contracts.Roles;

namespace SurveyBasket.Services.RoleServices;

public class RoleService(RoleManager<ApplicationRole> roleManager, ApplicationDbContext dbContext) : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<Result<IEnumerable<RolesResponse>>> GetAllAsync(bool? isDeleted, CancellationToken cancellationToken)
    {
        var roles = await _roleManager.Roles.Where(r => !r.IsDefault && (r.IsDeleted == isDeleted.GetValueOrDefault())).AsNoTracking()
            .ProjectToType<RolesResponse>().ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<RolesResponse>>(roles);
    }

    public async Task<Result<RoleDetialsResponse>> GetByIdAsync(string Id, CancellationToken cancellationToken = default)
    {
        if (await _roleManager.Roles.SingleOrDefaultAsync(r => r.Id == Id) is not { } role)
            return Result.Failure<RoleDetialsResponse>(new Error("RoleNotDefined", "role not defined", 404));
        // var role = _roleManager.Roles.Where(r => r.Id == Id).SingleOrDefaultAsync(cancellationToken: cancellationToken);

        var permissions = (await _roleManager.GetClaimsAsync(role)).Select(c => c.Value).ToList();

        var res = new RoleDetialsResponse(role.Id, role.Name!, role.IsDeleted, permissions);
        return Result.Success(res);
    }

    public async Task<Result<RoleDetialsResponse>> AddAsync(RoleRequest request)
    {
        var isRoleExsit = await _roleManager.RoleExistsAsync(request.Name);
        if (isRoleExsit)
            return Result.Failure<RoleDetialsResponse>(RoleError.DuplicatedRole);

        var rolesClaims = _dbContext.RoleClaims.Select(c => c.ClaimValue);
        if (request.Permissions.Except(rolesClaims).Any())
            return Result.Failure<RoleDetialsResponse>(RoleError.PermissionNotDefined);

        var roleRes = await _roleManager.CreateAsync(new ApplicationRole
        {
            Name = request.Name,
            ConcurrencyStamp = Guid.NewGuid().ToString()
        });

        if (roleRes.Succeeded)
        {
            var role = await _roleManager.FindByNameAsync(request.Name);
            var permissions = request.Permissions.Select(p => new IdentityRoleClaim<string>()
            {
                ClaimValue = p,
                RoleId = role!.Id,
                ClaimType = PermissionsClaims.Type
            }).ToList();
            await _dbContext.AddRangeAsync(permissions);
            await _dbContext.SaveChangesAsync();

            var result = new RoleDetialsResponse(role!.Id, role!.Name!, role.IsDeleted, request.Permissions);
            return Result.Success(result);
        }
        var error = roleRes.Errors.First();

        return Result.Failure<RoleDetialsResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result<RoleDetialsResponse>> UpdateAsync(string id, RoleRequest request, CancellationToken cancellationToken = default)
    {
        var isRoleWithSameName = await _roleManager.Roles.AnyAsync(r => r.Name == request.Name && r.Id != id);
        if (isRoleWithSameName)
            return Result.Failure<RoleDetialsResponse>(RoleError.DuplicatedRole);
        if (await _roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure<RoleDetialsResponse>(RoleError.RoleNotDefined);


        var rolesClaims = _dbContext.RoleClaims.Select(c => c.ClaimValue);
        if (request.Permissions.Except(rolesClaims).Any())
            return Result.Failure<RoleDetialsResponse>(RoleError.PermissionNotDefined);

        role.Name = request.Name;

        var roleRes = await _roleManager.UpdateAsync(role);

        if (roleRes.Succeeded)
        {
            var currentPermissions = await _dbContext.RoleClaims.Where(r => r.RoleId == id && r.ClaimType == PermissionsClaims.Type)
                .Select(r => r.ClaimValue).ToListAsync();
            var newPermissions = request.Permissions.Except(currentPermissions).Select(r => new IdentityRoleClaim<string>
            {
                ClaimType = PermissionsClaims.Type,
                ClaimValue = r,
                RoleId = id
            }).ToList();

            var removedClaims = currentPermissions.Except(request.Permissions).ToList();
            await _dbContext.RoleClaims.AddRangeAsync(newPermissions, cancellationToken);
            await _dbContext.RoleClaims.Where(c => c.ClaimType == PermissionsClaims.Type && removedClaims.Contains(c.ClaimValue) && c.RoleId == id)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            var result = new RoleDetialsResponse(role!.Id, role!.Name!, role.IsDeleted, request.Permissions);
            return Result.Success(result);
        }
        var error = roleRes.Errors.First();

        return Result.Failure<RoleDetialsResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
}
