using Application.Interfaces;

namespace Infrastructure.Services;

public class PaymentService : IPaymentService
{
	public async Task<PaymentResult> ProcessPaymentAsync(
		string cardNumber,
		string cardHolderName,
		string expiryMonth,
		string expiryYear,
		string cvv,
		decimal amount)
	{
		await Task.Delay(1000);

		if (cardNumber.Length < 13 || cardNumber.Length > 19)
		{
			return new PaymentResult
			{
				Success = false,
				Message = "Invalid card number"
			};
		}

		if (string.IsNullOrWhiteSpace(cardHolderName))
		{
			return new PaymentResult
			{
				Success = false,
				Message = "Card holder name is required"
			};
		}

		return new PaymentResult
		{
			Success = true,
			TransactionId = $"TXN-{Guid.NewGuid().ToString()[..12].ToUpper()}",
			Message = "Payment processed successfully"
		};
	}
}