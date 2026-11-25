using System.Text.Json.Serialization;

namespace PruebaTecnica.Application.Addresses.Commands.UpdateAddress;

public record UpdateAddressCommand(
    [property: JsonIgnore] int Id,
    string Street,
    string City,
    string Country,
    string? ZipCode);
