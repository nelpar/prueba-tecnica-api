using System.Text.Json.Serialization;

namespace PruebaTecnica.Application.Addresses.Commands.CreateAddress;

public record CreateAddressCommand(
    [property: JsonIgnore] int UserId,
    string Street,
    string City,
    string Country,
    string? ZipCode);
