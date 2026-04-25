namespace Ecommerce.Api.Contracts;

public record RegisterDto(string FirstName, string LastName, string PhoneNumber, string Email, string Password);
public record LoginDto(string Email, string Password);
public record AuthResponseDto(string Token, string Email, string UserId);
public record VerifyEmailDto(string UserId, string Token);
