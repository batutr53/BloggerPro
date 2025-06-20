using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BloggerPro.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<ICommentLikeService, CommentLikeService>();
        services.AddScoped<IPostRatingService, PostRatingService>();
        services.AddScoped<IPostLikeService, PostLikeService>();
        services.AddScoped<IUserPanelService, UserPanelService>();
        services.AddScoped<IAdminDashboardService, AdminDashboardService>();
        services.AddScoped<IUserAdminModerationService, UserAdminModerationService>();

        return services;
    }
}
