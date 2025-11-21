using GamesLoan.Api.DTOs.Auth;
using GamesLoan.Application.Auth.Commands.LoginUser;
using GamesLoan.Application.Auth.Commands.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamesLoan.Api.Controllers;

/// <summary>
/// Gerencia autenticação e registro de usuários.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registra um novo usuário.
    /// </summary>
    /// <param name="request">Dados de registro (username e password).</param>
    /// <returns>Id e username do usuário criado.</returns>
    /// <response code="201">Usuário criado com sucesso.</response>
    /// <response code="409">Username já existe.</response>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var id = await _mediator.Send(new RegisterUserCommand(request.Username, request.Password));
        return Created(string.Empty, new { id, username = request.Username });
    }

    /// <summary>
    /// Realiza login e retorna um token JWT.
    /// </summary>
    /// <param name="request">Dados de login (username e password).</param>
    /// <returns>Token JWT e username.</returns>
    /// <response code="200">Login efetuado.</response>
    /// <response code="404">Usuário não encontrado ou credenciais inválidas.</response>
    /// <response code="401">Credenciais inválidas.</response>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var response = await _mediator.Send(new LoginUserCommand(request.Username, request.Password));
        return Ok(response);
    }
}