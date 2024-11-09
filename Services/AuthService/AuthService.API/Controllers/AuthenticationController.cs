using Asp.Versioning;
using AuthService.Application.Models.Requests;
using AuthService.Application.UseCases.v1.Commands.Login;
using AuthService.Application.UseCases.v1.Commands.Register;
using AuthService.Application.UseCases.v1.Commands.RenewToken;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Response;

namespace AuthService.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{api_version:apiVersion}/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new LoginCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.Status, response.ErrorMessageCode, response.Data);
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new RegisterCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.Status, response.ErrorMessageCode);
    }

    [HttpPost]
    [Authorize]
    [Route("renew-token")]
    public async Task<IActionResult> RenewToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new RenewTokenCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.Status, response.ErrorMessageCode, response.Data);
    }

    //Test api versioning
    [HttpPost]
    [Authorize]
    [ApiVersion("2.0")]
    [Route("renew-token")]
    public async Task<IActionResult> RenewTokenV2([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new RenewTokenCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.Status, response.ErrorMessageCode, response.Data);
    }
}