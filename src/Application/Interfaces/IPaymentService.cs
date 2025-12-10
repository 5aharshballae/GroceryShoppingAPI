namespace Application.Interfaces;

public interface IPaymentService
{
	Task<PaymentResult> ProcessPaymentAsync(
		string cardNumber,
		string cardHolderName,
		string expiryMonth,
		string expiryYear,
		string cvv,
		decimal amount
	);
}

public class PaymentResult
{
	public bool Success { get; set; }
	public string TransactionId { get; set; } = string.Empty;
	public string Message { get; set; } = string.Empty;
}