namespace Domain.Entities;

public class Order
{
	public Guid Id { get; set; }
	public string OrderNumber { get; set; } = string.Empty;
	public Guid UserId { get; set; }
	public User User { get; set; } = null!;
	public decimal TotalAmount { get; set; }
	public OrderStatus Status { get; set; }
	public string PaymentIntentId { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
	public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

public enum OrderStatus
{
	Pending,
	Processing,
	Completed,
	Cancelled,
	Failed
}