using FluentValidation.Validators;
using FluentValidation;
using System.Net;

namespace Ecommerce.Api.Validators;

public class EmailDomainValidator<T> : AsyncPropertyValidator<T, string>
{
    public override string Name => "EmailDomainValidator";

    public override async Task<bool> IsValidAsync(ValidationContext<T> context, string value, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
        {
            return false;
        }

        var domain = value.Split('@').Last();
        try
        {
            var hostEntry = await Dns.GetHostEntryAsync(domain);
            // A simple check for any address. A more robust check might look for MX records specifically.
            return hostEntry.AddressList.Any();
        }
        catch (Exception)
        {
            return false;
        }
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
        => "'{PropertyName}' must be a valid and resolvable email domain.";
}