using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharedKernel;

namespace Infrastructure.Configuration;

public class FluentValidateOptions<TOptions> 
    : IValidateOptions<TOptions>
    where TOptions : class
{
    private readonly IServiceProvider _serviceProvider;
    private readonly string? _name;

    public FluentValidateOptions(IServiceProvider serviceProvider, string? name)
    {
        _serviceProvider = serviceProvider;
        _name = name;
    }

    public ValidateOptionsResult Validate(string? name, TOptions options)
    {
        if (!string.IsNullOrWhiteSpace(_name) && _name != name)
            return ValidateOptionsResult.Skip;
        
        Ensure.NotNull(options);

        using var scope = _serviceProvider.CreateScope();

        var validator = scope.ServiceProvider.GetRequiredService<IValidator<TOptions>>();

        var result = validator.Validate(options);

        if (result.IsValid)
            return ValidateOptionsResult.Success;

        var type = options.GetType().Name;
        var errors = result.Errors.Select(failure =>
            $"Validation failed for {type}.{failure.PropertyName} with the error {failure.ErrorMessage}").ToList();

        return ValidateOptionsResult.Fail(errors);
    }
}