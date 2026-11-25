using PruebaTecnica.Api.Filters;
using PruebaTecnica.Application.Common.Cqrs;
using PruebaTecnica.Application.Currencies.Commands.CreateCurrency;
using PruebaTecnica.Application.Currencies.Dtos;
using PruebaTecnica.Application.Currencies.Queries.GetCurrencies;
using PruebaTecnica.Application.CurrencyConversion;

namespace PruebaTecnica.Api.Endpoints;

public static class CurrencyEndpoints
{
    public static IEndpointRouteBuilder MapCurrencyEndpoints(this IEndpointRouteBuilder app)
    {
        var currencies = app.MapGroup("/currencies").WithTags("Currencies");

        currencies.MapGet("/", async (
            IQueryHandler<GetCurrenciesQuery, List<CurrencyResponse>> handler,
            CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetCurrenciesQuery(), ct)));

        currencies.MapPost("/", async (
            CreateCurrencyCommand command,
            ICommandHandler<CreateCurrencyCommand, CurrencyResponse> handler,
            CancellationToken ct) =>
        {
            var currency = await handler.Handle(command, ct);
            return Results.Created("/currencies", currency);
        })
        .AddEndpointFilter<ValidationFilter<CreateCurrencyCommand>>();

        app.MapPost("/currency/convert", async (
            ConvertCurrencyQuery query,
            IQueryHandler<ConvertCurrencyQuery, ConversionResponse> handler,
            CancellationToken ct) =>
            Results.Ok(await handler.Handle(query, ct)))
        .WithTags("Currency Conversion")
        .AddEndpointFilter<ValidationFilter<ConvertCurrencyQuery>>();

        return app;
    }
}
