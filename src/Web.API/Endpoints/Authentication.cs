using Application.Features.Authentication.Commands.Login;
using Carter;
using MediatR;
using Web.API.Contracts;
using Web.API.Extensions;
using Web.API.Infrastructure;

namespace Web.API.Endpoints;

public sealed class Authentication : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var authentication = app
            .MapGroup(ApiRoutes.Authentication.BaseUri)
            .WithTags(ApiRoutes.Authentication.Tag);

        authentication.MapPost(ApiRoutes.Authentication.Login, async (
            LoginRequest request,
            ISender sender) =>
        {
            var command = new LoginCommand(request.Email, request.Password);

            var result = await sender.Send(command);

            return result.Match(Results.Ok, CustomResults.Problem);
        });
    }
}