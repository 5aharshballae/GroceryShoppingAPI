using Infrastructure.Services;
using Xunit;

namespace GroceryAPI.Tests;

public class PaymentServiceTests
{
	private readonly PaymentService _paymentService;

	public PaymentServiceTests()
	{
		_paymentService = new PaymentService();
	}

	[Fact]
	public async Task ProcessPayment_WithValidCard_ReturnsSuccess()
	{
		// Arrange & Act
		var result = await _paymentService.ProcessPaymentAsync(
			"4111111111111111",
			"John Doe",
			"12",
			"2025",
			"123",
			100.00m
		);

		// Assert
		Assert.True(result.Success);
		Assert.NotEmpty(result.TransactionId);
		Assert.Equal("Payment processed successfully", result.Message);
	}

	[Fact]
	public async Task ProcessPayment_WithInvalidCard_ReturnsFailure()
	{
		// Arrange & Act
		var result = await _paymentService.ProcessPaymentAsync(
			"123",
			"John Doe",
			"12",
			"2025",
			"123",
			100.00m
		);

		// Assert
		Assert.False(result.Success);
		Assert.Equal("Invalid card number", result.Message);
	}

	[Fact]
	public async Task ProcessPayment_WithEmptyCardHolder_ReturnsFailure()
	{
		// Arrange & Act
		var result = await _paymentService.ProcessPaymentAsync(
			"4111111111111111",
			"",
			"12",
			"2025",
			"123",
			100.00m
		);

		// Assert
		Assert.False(result.Success);
		Assert.Equal("Card holder name is required", result.Message);
	}
}