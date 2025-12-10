using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
	private readonly ILogger<EmailService> _logger;

	public EmailService(ILogger<EmailService> logger)
	{
		_logger = logger;
	}

	public async Task SendOrderConfirmationAsync(string email, string orderNumber, decimal totalAmount)
	{
		await Task.Delay(500);

		var emailBody = $@"
Thank you for your order!

Order Number: {orderNumber}
Total Amount: ${totalAmount:F2}

We'll process your order shortly.

Best regards,
Grocery Shopping Team
        ";

		_logger.LogInformation($"Sending order confirmation email to {email} for order {orderNumber}");

		Console.WriteLine("========================================");
		Console.WriteLine($"[EMAIL SENT] To: {email}");
		Console.WriteLine($"[EMAIL SENT] Subject: Order Confirmation - {orderNumber}");
		Console.WriteLine($"[EMAIL SENT] Body:\n{emailBody}");
		Console.WriteLine("========================================");
	}
}