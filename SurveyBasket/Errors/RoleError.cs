namespace SurveyBasket.Errors;

public record RoleError
{
    public static Error DuplicatedRole => new("DuplicatedRole", "Role defined before", StatusCodes.Status409Conflict);

    public static Error PermissionNotDefined => new("PermissionNotDefined", "Permission Not Defined", StatusCodes.Status400BadRequest);
    public static Error DuplicatedPermission => new("DuplicatedPermission", "Permission defined twice or more", StatusCodes.Status400BadRequest);
    public static Error RoleNotDefined => new("RoleNotDefined", "Role Not Defined", StatusCodes.Status400BadRequest);


}