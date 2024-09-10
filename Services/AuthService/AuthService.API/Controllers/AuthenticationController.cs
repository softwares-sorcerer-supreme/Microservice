using Asp.Versioning;
using AuthService.Application.Models.Requests;
using AuthService.Application.UseCases.v1.Commands.Login;
using AuthService.Application.UseCases.v1.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Response;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiVersion("1.0")]
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
    
    
    // [HttpPost]
    // [Route("refresh-token")]
    //
    // public async Task<IActionResult> RefreshToken(Guid id, CancellationToken cancellationToken)
    // {
    //     var response = await _mediator.Send(new GetItemsByCartIdQuery(id), cancellationToken);
    //     return ResponseHelper.ToResponse(response.Status, response.ErrorMessageCode, response.Data);
    // }
    
    
    
}