namespace Application.DTOs;

public record StoreDto(
	Guid Id,
	string Name,
	string Description,
	string Address,
	bool IsActive
);

public record CategoryDto(
	Guid Id,
	string Name,
	string Description,
	Guid StoreId
);