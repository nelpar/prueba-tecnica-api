using System.Text.Json.Serialization;

namespace PruebaTecnica.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    [property: JsonIgnore] int Id,
    string Name,
    string Email,
    bool? IsActive = null);