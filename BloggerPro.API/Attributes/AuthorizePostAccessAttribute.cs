using BloggerPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace BloggerPro.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class AuthorizePostAccessAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _requiredRoles;
        private readonly bool _allowOwners;

        public AuthorizePostAccessAttribute(bool allowOwners = true, params string[] roles)
        {
            _requiredRoles = roles;
            _allowOwners = allowOwners;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousAttribute)))
                return;

            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Role kontrolü
            if (_requiredRoles != null && _requiredRoles.Any())
            {
                var hasRequiredRole = _requiredRoles.Any(role => user.IsInRole(role));
                if (hasRequiredRole)
                    return;
            }

            // Sahiplik kontrolü
            if (_allowOwners)
            {
                var userIdStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var postIdStr = context.RouteData.Values["id"]?.ToString();

                if (Guid.TryParse(userIdStr, out var userId) && Guid.TryParse(postIdStr, out var postId))
                {
                    var postService = (IPostService)context.HttpContext.RequestServices.GetService(typeof(IPostService));
                    if (postService != null)
                    {
                        var isOwner = await postService.IsPostOwnerAsync(postId, userId);
                        if (isOwner)
                            return;
                    }
                }
            }

            context.Result = new ForbidResult();
        }

    }
}
