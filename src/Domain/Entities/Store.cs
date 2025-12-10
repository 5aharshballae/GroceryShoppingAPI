namespace Domain.Entities;

public class Store
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string Address { get; set; } = string.Empty;
	public bool IsActive { get; set; }
	public ICollection<Category> Categories { get; set; } = new List<Category>();
}