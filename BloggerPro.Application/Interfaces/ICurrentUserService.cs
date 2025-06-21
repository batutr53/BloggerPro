using System;
using System.Security.Claims;

namespace BloggerPro.Application.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string UserName { get; }
        string Email { get; }
        bool IsAuthenticated { get; }
        bool IsInRole(string role);
        bool HasPermission(string permission);
        ClaimsPrincipal GetUserClaimsPrincipal();
    }
}
