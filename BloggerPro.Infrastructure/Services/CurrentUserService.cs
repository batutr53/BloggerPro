using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using BloggerPro.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BloggerPro.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;

        public Guid? UserId
        {
            get
            {
                var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    return null;
                return userId;
            }
        }

        public string UserName => User?.FindFirst(ClaimTypes.Name)?.Value;

        public string Email => User?.FindFirst(ClaimTypes.Email)?.Value;

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        public bool IsInRole(string role)
        {
            return User?.IsInRole(role) ?? false;
        }

        public bool HasPermission(string permission)
        {
            return User?.HasClaim("permission", permission) ?? false;
        }

        public ClaimsPrincipal GetUserClaimsPrincipal()
        {
            return User;
        }

        public async Task<bool> IsInRoleAsync(string role)
        {
            return await Task.FromResult(IsInRole(role));
        }

        public async Task<bool> HasPermissionAsync(string permission)
        {
            return await Task.FromResult(HasPermission(permission));
        }

        public string GetClaimValue(string claimType)
        {
            return User?.FindFirst(claimType)?.Value;
        }

        public IEnumerable<string> GetClaimValues(string claimType)
        {
            return User?.FindAll(claimType)?.Select(c => c.Value) ?? Enumerable.Empty<string>();
        }
    }
}
