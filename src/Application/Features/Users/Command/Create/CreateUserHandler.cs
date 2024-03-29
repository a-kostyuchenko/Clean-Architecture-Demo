using Application.Abstractions;
using Application.Abstractions.Cryptography;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Roles;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Users.Command.Create;

internal sealed class CreateUserHandler(
    IApplicationDbContext context,
    IPasswordHasher passwordHasher) : ICommandHandler<CreateUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var firstNameResult = FirstName.Create(request.FirstName);
        var lastNameResult = LastName.Create(request.LastName);
        var emailResult = Email.Create(request.Email);
        var passwordResult = Password.Create(request.Password);

        Result firstFailureOrSuccess = Result.FirstFailureOrSuccess(
            firstNameResult,
            lastNameResult,
            emailResult,
            passwordResult);

        if (firstFailureOrSuccess.IsFailure)
            return Result.Failure<Guid>(firstFailureOrSuccess.Error);

        if (await context.Users.AnyAsync(x => x.Email == emailResult.Value, cancellationToken))
            return Result.Failure<Guid>(UserErrors.EmailAlreadyInUse);
        
        string passwordHash = passwordHasher.HashPassword(passwordResult.Value);

        var user = User.Create(firstNameResult.Value, lastNameResult.Value, emailResult.Value, passwordHash);
        
        context.Insert(user);
        
        return Result.Success(user.Id);
    }
}