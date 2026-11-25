using PruebaTecnica.Api.Filters;
using PruebaTecnica.Application.Common.Cqrs;
using PruebaTecnica.Application.Users.Commands.CreateUser;
using PruebaTecnica.Application.Users.Commands.DeleteUser;
using PruebaTecnica.Application.Users.Commands.UpdateUser;
using PruebaTecnica.Application.Users.Dtos;
using PruebaTecnica.Application.Users.Queries.GetUserById;
using PruebaTecnica.Application.Users.Queries.GetUsers;

namespace PruebaTecnica.Api.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users").WithTags("Users");

        group.MapGet("/", async (
            bool? isActive,
            IQueryHandler<GetUsersQuery, List<UserResponse>> handler,
            CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetUsersQuery(isActive), ct)));

        group.MapGet("/{id:int}", async (
            int id,
            IQueryHandler<GetUserByIdQuery, UserResponse> handler,
            CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetUserByIdQuery(id), ct)));

        group.MapPost("/", async (
            CreateUserCommand command,
            ICommandHandler<CreateUserCommand, UserResponse> handler,
            CancellationToken ct) =>
        {
            var user = await handler.Handle(command, ct);
            return Results.Created($"/users/{user.Id}", user);
        })
        .AddEndpointFilter<ValidationFilter<CreateUserCommand>>();

        group.MapPut("/{id:int}", async (
            int id,
            UpdateUserCommand command,
            ICommandHandler<UpdateUserCommand, UserResponse> handler,
            CancellationToken ct) =>
            Results.Ok(await handler.Handle(command with { Id = id }, ct)))
        .AddEndpointFilter<ValidationFilter<UpdateUserCommand>>();

        group.MapDelete("/{id:int}", async (
            int id,
            ICommandHandler<DeleteUserCommand> handler,
            CancellationToken ct) =>
        {
            await handler.Handle(new DeleteUserCommand(id), ct);
            return Results.NoContent();
        });

        return app;
    }
}
