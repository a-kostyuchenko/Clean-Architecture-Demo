using Application.Features.Users.Command.ChangePassword;
using Application.Features.Users.Command.Create;
using Application.Features.Users.Queries.GetById;
using Carter;
using Domain;
using Mapster;
using MediatR;
using Web.API.Contracts;
using Web.API.Extensions;
using Web.API.Infrastructure;

namespace Web.API.Endpoints;

public sealed class Users : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var users = app
            .MapGroup(ApiRoutes.Users.BaseUri)
            .WithTags(ApiRoutes.Users.Tag);

        users.MapGet(ApiRoutes.Users.GetById, async (
            Guid userId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUserByIdQuery(userId);
            
            var result = await sender.Send(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        });

        users.MapPost(string.Empty, async (
            CreateUserRequest request,
            ISender sender) =>
        {
            var command = request.Adapt<CreateUserCommand>();
            
            var result = await sender.Send(command);
            
            return result.Match(Results.Created, CustomResults.Problem);
        });

        users.MapPut(ApiRoutes.Users.ChangePassword, async (
            Guid userId,
            ChangePasswordRequest request,
            ISender sender) =>
        {
            var command = new ChangePasswordCommand(userId, request.Password);

            var result = await sender.Send(command);
            
            return result.Match(Results.NoContent, CustomResults.Problem);
        });
    }
}