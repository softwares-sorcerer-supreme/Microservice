
using System.Text.Json;
using AuthService.Application.Abstraction.Interfaces;
using AuthService.Application.Models.Dtos;
using AuthService.Application.Models.Responses.Services;
using AuthService.Infrastructure.Extensions;
using AuthService.Persistence.Identity;
using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Shared.CommonExtension;
using Shared.Constants;
using Shared.Enums;

namespace AuthService.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly HttpClient _httpClient;
    private readonly ILogger<IdentityService> _logger;
    private readonly RoleManager<IdentityRole> _roleManager;
    
    public IdentityService
    (
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        HttpClient httpClient,
        ILogger<IdentityService> logger,
        RoleManager<IdentityRole> roleManager
    )
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _httpClient = httpClient;
        _logger = logger;
        _roleManager = roleManager;
    }

    public async Task<SignInServiceResponse> SignInAsync(string email, string password, bool isPersistent, bool lockoutOnFailure)
    {
        const string functionName = $"{nameof(IdentityService)} - {nameof(SignInAsync)} => ";
        _logger.LogInformation($"{functionName} Email = {email}");
        var signInResponse = new SignInServiceResponse();

        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning($"{functionName} User not found");
                signInResponse.HasError = true;
                signInResponse.Status = ResponseStatusCode.NotFound.ToInt();
                signInResponse.ErrorMessageCode = ResponseErrorMessageCode.ERR_AUTH_0001;
                return signInResponse;
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
            if (!result.Succeeded)
            {
                _logger.LogWarning($"{functionName} Invalid email or password");
                signInResponse.HasError = true;
                signInResponse.Status = ResponseStatusCode.BadRequest.ToInt();
                signInResponse.ErrorMessageCode = ResponseErrorMessageCode.ERR_AUTH_0002;
                return signInResponse;
            }

            signInResponse.Data = new SignInServiceData
            {
                User = user.ToUserDto()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"{functionName} has Error: {ex.Message}");
            signInResponse.HasError = true;
            signInResponse.Status = ResponseStatusCode.InternalServerError.ToInt();
            signInResponse.ErrorMessageCode = ResponseErrorMessageCode.ERR_SYS_0001;
            return signInResponse;
        }

        return signInResponse;
    }

    public async Task<TokenResponse> GetTokenFromCredential(PasswordTokenRequest request, CancellationToken cancellationToken)
    {
        return await _httpClient.RequestPasswordTokenAsync(request, cancellationToken);
    }
    
    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<RegisterServiceResponse> RegisterAsync(UserDto userDto, string password)
    {
        const string functionName = $"{nameof(IdentityService)} - {nameof(RegisterAsync)} => ";
        _logger.LogInformation($"{functionName} Email = {userDto.Email}");
        var response = new RegisterServiceResponse
        {
            HasError = true,
        };
        
        try
        {
            var user = await _userManager.FindByEmailAsync(userDto.Email);
            if(user != null)
            {
                _logger.LogWarning($"{functionName} User already exists");
                response.Status = ResponseStatusCode.BadRequest.ToInt();
                response.ErrorMessageCode = ResponseErrorMessageCode.ERR_AUTH_0003;
                return response;
            }

            user = new ApplicationUser
            {
                UserName = userDto.Email,
                Email = userDto.Email,
                FullName = userDto.FullName,
                PhoneNumber = userDto.PhoneNumber,
                EmailConfirmed = userDto.IsEmailVerified
            };

            var createUserResult = await _userManager.CreateAsync(user, password);

            if (!createUserResult.Succeeded)
            {
                _logger.LogError($"{functionName} Error while creating user: {JsonSerializer.Serialize(createUserResult.Errors)}");
                response.Status = ResponseStatusCode.InternalServerError.ToInt();
                response.ErrorMessageCode = ResponseErrorMessageCode.ERR_SYS_0001;
            }

            var roleStr = userDto.Role;
            if (string.IsNullOrEmpty(roleStr))
            {
                roleStr = RoleConst.Guest;
            }
            
            var role = await _roleManager.FindByNameAsync(roleStr);
            if (role == null)
            {
                _logger.LogWarning($"{functionName} Role not found");
                response.Status = ResponseStatusCode.NotFound.ToInt();
                response.ErrorMessageCode = ResponseErrorMessageCode.ERR_AUTH_0008;
                return response;
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, role.Name);
            if (!addRoleResult.Succeeded)
            {
                _logger.LogError($"{functionName} Error while adding role to user: {JsonSerializer.Serialize(addRoleResult.Errors)}");
                response.Status = ResponseStatusCode.InternalServerError.ToInt();
                response.ErrorMessageCode = ResponseErrorMessageCode.ERR_SYS_0001;
                return response;
            }
            
            response.HasError = false;
            
        }
        catch (Exception e)
        {
            _logger.LogError($"{functionName} has error: {e.Message}");
            response.Status = ResponseStatusCode.InternalServerError.ToInt();
            response.ErrorMessageCode = ResponseErrorMessageCode.ERR_SYS_0001;
        }

        return response;
    }

    public Task<string> RefreshTokenAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }
}