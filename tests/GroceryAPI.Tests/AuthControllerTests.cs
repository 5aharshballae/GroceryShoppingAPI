using Application.Commands;
using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using GroceryAPI.Controllers;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace GroceryAPI.Tests;

public class AuthControllerTests
{
	private readonly Mock<RegisterUserCommandHandler> _mockRegisterHandler;
	private readonly Mock<IUserRepository> _mockUserRepository;
	private readonly Mock<TokenService> _mockTokenService;
	private readonly AuthController _controller;

	public AuthControllerTests()
	{
		_mockRegisterHandler = new Mock<RegisterUserCommandHandler>(
			Mock.Of<IUserRepository>(),
			Mock.Of<ICartRepository>()
		);
		_mockUserRepository = new Mock<IUserRepository>();
		_mockTokenService = new Mock<TokenService>(Mock.Of<IConfiguration>());

		_controller = new AuthController(
			_mockRegisterHandler.Object,
			_mockUserRepository.Object,
			_mockTokenService.Object
		);
	}

	[Fact]
	public async Task Register_WithValidData_ReturnsOkResult()
	{
		// Arrange
		var request = new RegisterRequest(
			"test@example.com",
			"password123",
			"John",
			"Doe"
		);

		var user = new User
		{
			Id = Guid.NewGuid(),
			Email = request.Email,
			FirstName = request.FirstName,
			LastName = request.LastName
		};

		_mockRegisterHandler
			.Setup(h => h.HandleAsync(It.IsAny<RegisterUserCommand>()))
			.ReturnsAsync(user);

		_mockTokenService
			.Setup(s => s.GenerateToken(user))
			.Returns("fake-jwt-token");

		// Act
		var result = await _controller.Register(request);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result.Result);
		var response = Assert.IsType<AuthResponse>(okResult.Value);
		Assert.Equal(user.Email, response.Email);
	}

	[Fact]
	public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
	{
		// Arrange
		var request = new LoginRequest("test@example.com", "wrongpassword");

		_mockUserRepository
			.Setup(r => r.GetByEmailAsync(request.Email))
			.ReturnsAsync((User?)null);

		// Act
		var result = await _controller.Login(request);

		// Assert
		Assert.IsType<UnauthorizedObjectResult>(result.Result);
	}

	[Fact]
	public async Task Login_WithValidCredentials_ReturnsToken()
	{
		// Arrange
		var password = "password123";
		var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

		var user = new User
		{
			Id = Guid.NewGuid(),
			Email = "test@example.com",
			PasswordHash = hashedPassword,
			FirstName = "John",
			LastName = "Doe"
		};

		var request = new LoginRequest(user.Email, password);

		_mockUserRepository
			.Setup(r => r.GetByEmailAsync(request.Email))
			.ReturnsAsync(user);

		_mockTokenService
			.Setup(s => s.GenerateToken(user))
			.Returns("fake-jwt-token");

		// Act
		var result = await _controller.Login(request);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result.Result);
		var response = Assert.IsType<AuthResponse>(okResult.Value);
		Assert.Equal(user.Email, response.Email);
		Assert.NotEmpty(response.Token);
	}
}