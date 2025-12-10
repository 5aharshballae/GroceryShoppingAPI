namespace Application.Interfaces;

public interface IEmailService
{
	Task SendOrderConfirmationAsync(string email, string orderNumber, decimal totalAmount);
}