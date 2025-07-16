using BloggerPro.Application.Interfaces;
using BloggerPro.Application.Interfaces.Services;
using BloggerPro.Domain.Entities;
using BloggerPro.Infrastructure.Services;
using Ganss.Xss;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BloggerPro.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Add HTTP context accessor
        services.AddHttpContextAccessor();

        // Add current user service
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Add AutoMapper
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // Register services
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
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IUserFollowerService, UserFollowerService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IAdminModerationService, AdminModerationService>();
        services.AddScoped<IContactService, ContactService>();
        services.AddSingleton<IHtmlSanitizer, HtmlSanitizer>();
        return services;
    }
}
