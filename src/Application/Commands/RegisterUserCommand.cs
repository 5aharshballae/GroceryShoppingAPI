using Domain.Entities;
using Domain.Interfaces;

namespace Application.Commands;

public class RegisterUserCommand
{
	public string Email { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
}

public class RegisterUserCommandHandler
{
	private readonly IUserRepository _userRepository;
	private readonly ICartRepository _cartRepository;

	public RegisterUserCommandHandler(IUserRepository userRepository, ICartRepository cartRepository)
	{
		_userRepository = userRepository;
		_cartRepository = cartRepository;
	}

	public async Task<User> HandleAsync(RegisterUserCommand command)
	{
		if (await _userRepository.ExistsAsync(command.Email))
		{
			throw new InvalidOperationException("User with this email already exists");
		}

		var user = new User
		{
			Id = Guid.NewGuid(),
			Email = command.Email,
			PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
			FirstName = command.FirstName,
			LastName = command.LastName,
			CreatedAt = DateTime.UtcNow
		};

		await _userRepository.AddAsync(user);

		var cart = new Cart
		{
			Id = Guid.NewGuid(),
			UserId = user.Id,
			CreatedAt = DateTime.UtcNow,
			UpdatedAt = DateTime.UtcNow
		};

		await _cartRepository.AddAsync(cart);
		await _userRepository.SaveChangesAsync();

		return user;
	}
}