using PruebaTecnica.Api.Filters;
using PruebaTecnica.Application.Addresses.Commands.CreateAddress;
using PruebaTecnica.Application.Addresses.Commands.DeleteAddress;
using PruebaTecnica.Application.Addresses.Commands.UpdateAddress;
using PruebaTecnica.Application.Addresses.Dtos;
using PruebaTecnica.Application.Addresses.Queries.GetAddressesByUser;
using PruebaTecnica.Application.Common.Cqrs;

namespace PruebaTecnica.Api.Endpoints;

public static class AddressEndpoints
{
    public static IEndpointRouteBuilder MapAddressEndpoints(this IEndpointRouteBuilder app)
    {
        var byUser = app.MapGroup("/users/{userId:int}/addresses").WithTags("Addresses");

        byUser.MapGet("/", async (
            int userId,
            IQueryHandler<GetAddressesByUserQuery, List<AddressResponse>> handler,
            CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetAddressesByUserQuery(userId), ct)));

        byUser.MapPost("/", async (
            int userId,
            CreateAddressCommand command,
            ICommandHandler<CreateAddressCommand, AddressResponse> handler,
            CancellationToken ct) =>
        {
            var address = await handler.Handle(command with { UserId = userId }, ct);
            return Results.Created($"/users/{userId}/addresses", address);
        })
        .AddEndpointFilter<ValidationFilter<CreateAddressCommand>>();

        var addresses = app.MapGroup("/addresses").WithTags("Addresses");

        addresses.MapPut("/{id:int}", async (
            int id,
            UpdateAddressCommand command,
            ICommandHandler<UpdateAddressCommand, AddressResponse> handler,
            CancellationToken ct) =>
            Results.Ok(await handler.Handle(command with { Id = id }, ct)))
        .AddEndpointFilter<ValidationFilter<UpdateAddressCommand>>();

        addresses.MapDelete("/{id:int}", async (
            int id,
            ICommandHandler<DeleteAddressCommand> handler,
            CancellationToken ct) =>
        {
            await handler.Handle(new DeleteAddressCommand(id), ct);
            return Results.NoContent();
        });

        return app;
    }
}
