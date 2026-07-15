using Abp.Authorization;
using Nexora.Authorization.Roles;
using Nexora.Authorization.Users;

namespace Nexora.Authorization;

public class PermissionChecker : PermissionChecker<Role, User>
{
    public PermissionChecker(UserManager userManager)
        : base(userManager)
    {
    }
}
