namespace Application.Features.Users.Queries.GetById;

public sealed record UserResponse
{
    public Guid Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
}