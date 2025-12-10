namespace Application.DTOs;

public record RegisterRequest(
	string Email,
	string Password,
	string FirstName,
	string LastName
);

public record LoginRequest(
	string Email,
	string Password
);

public record AuthResponse(
	Guid UserId,
	string Email,
	string FirstName,
	string LastName,
	string Token
);