using GamesLoan.Api.DTOs.Auth;
using GamesLoan.Application.Auth.Commands.LoginUser;
using GamesLoan.Application.Auth.Commands.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamesLoan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var id = await _mediator.Send(new RegisterUserCommand(request.Username, request.Password));
        return Created(string.Empty, new { id, username = request.Username });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var response = await _mediator.Send(new LoginUserCommand(request.Username, request.Password));
        return Ok(response);
    }
}