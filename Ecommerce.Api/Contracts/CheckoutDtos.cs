namespace Ecommerce.Api.Contracts;

public record CartItemDto(int ProductId, int Quantity);
public record CheckoutRequestDto(List<CartItemDto> Items);
public record CheckoutResponseDto(string Url);