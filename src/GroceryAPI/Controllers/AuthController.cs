using Application.Commands;
using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace GroceryAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	private readonly RegisterUserCommandHandler _registerHandler;
	private readonly IUserRepository _userRepository;
	private readonly TokenService _tokenService;

	public AuthController(
		RegisterUserCommandHandler registerHandler,
		IUserRepository userRepository,
		TokenService tokenService)
	{
		_registerHandler = registerHandler;
		_userRepository = userRepository;
		_tokenService = tokenService;
	}

	[HttpPost("register")]
	public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
	{
		try
		{
			var command = new RegisterUserCommand
			{
				Email = request.Email,
				Password = request.Password,
				FirstName = request.FirstName,
				LastName = request.LastName
			};

			var user = await _registerHandler.HandleAsync(command);
			var token = _tokenService.GenerateToken(user);

			return Ok(new AuthResponse(
				user.Id,
				user.Email,
				user.FirstName,
				user.LastName,
				token
			));
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	[HttpPost("login")]
	public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
	{
		var user = await _userRepository.GetByEmailAsync(request.Email);

		if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
		{
			return Unauthorized(new { message = "Invalid email or password" });
		}

		var token = _tokenService.GenerateToken(user);

		return Ok(new AuthResponse(
			user.Id,
			user.Email,
			user.FirstName,
			user.LastName,
			token
		));
	}
}