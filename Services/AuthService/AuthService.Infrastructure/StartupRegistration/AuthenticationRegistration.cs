using AuthService.Infrastructure.Identity;
using AuthService.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Infrastructure.StartupRegistration;

public static class AuthenticationRegistration
{
    public static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddApiEndpoints();

        services.Configure<IdentityOptions> (options => {
            // Thiết lập về Password
            options.Password.RequireDigit = false; // Không bắt phải có số
            options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
            options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
            options.Password.RequireUppercase = false; // Không bắt buộc chữ in
            options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
            options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

            // Cấu hình Lockout - khóa user
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes (5); // Khóa 5 phút
            options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
            options.Lockout.AllowedForNewUsers = true;

            // Cấu hình về User.
            options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true; // Email là duy nhất

            // Cấu hình đăng nhập.
            options.SignIn.RequireConfirmedEmail = true; // Cấu hình xác thực địa chỉ email (email phải tồn tại)
            options.SignIn.RequireConfirmedPhoneNumber = false; // Xác thực số điện thoại
        });

        services
            .AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
                options.EmitStaticAudienceClaim = true;
            })
            // .AddInMemoryIdentityResources(Config.IdentityResources) // Use this when use OIDC 
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients(configuration)) //TODO: change this => .AddClientStore<>()
            .AddAspNetIdentity<ApplicationUser>()
            .AddDeveloperSigningCredential()
            .AddProfileService<ProfileService>()
            .AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator>();

        // builder.Services.AddAuthentication()
        //     .AddGoogle(options =>
        //     {
        //         options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        //
        //         // register your IdentityServer with Google at https://console.developers.google.com
        //         // enable the Google+ API
        //         // set the redirect URI to https://localhost:5001/signin-google
        //         options.ClientId = "copy client ID from Google here";
        //         options.ClientSecret = "copy client secret from Google here";
        //     });
        
        return services;
    }
}