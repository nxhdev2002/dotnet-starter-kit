﻿using FSH.Framework.Core.Identity.Tokens;
using FSH.Framework.Core.Identity.Users.Abstractions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Identity.Persistence;
using FSH.Framework.Infrastructure.Identity.Roles;
using FSH.Framework.Infrastructure.Identity.Tokens;
using FSH.Framework.Infrastructure.Identity.Tokens.Endpoints;
using FSH.Framework.Infrastructure.Identity.Users;
using FSH.Framework.Infrastructure.Identity.Users.Endpoints;
using FSH.Framework.Infrastructure.Identity.Users.Services;
using FSH.Framework.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Framework.Infrastructure.Identity;
internal static class Extensions
{
    internal static IServiceCollection ConfigureIdentity(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddTransient<ICurrentUser, CurrentUser>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<ITokenService, TokenService>();
        services.BindDbContext<IdentityDbContext>();
        services.AddScoped<IDbInitializer, IdentityDbInitializer>();
        return services
           .AddIdentity<FshUser, FshRole>(options =>
           {
               options.Password.RequiredLength = IdentityConstants.PasswordLength;
               options.Password.RequireDigit = false;
               options.Password.RequireLowercase = false;
               options.Password.RequireNonAlphanumeric = false;
               options.Password.RequireUppercase = false;
               options.User.RequireUniqueEmail = true;
           })
           .AddEntityFrameworkStores<IdentityDbContext>()
           .AddDefaultTokenProviders()
           .Services;
    }

    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("api/users").WithTags("users");
        users.MapUserEndpoints();

        var tokens = app.MapGroup("api/token").WithTags("token");
        tokens.MapTokenEndpoints();
        return app;
    }
}
