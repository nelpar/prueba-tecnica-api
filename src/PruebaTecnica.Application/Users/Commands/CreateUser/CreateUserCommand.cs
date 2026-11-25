namespace PruebaTecnica.Application.Users.Commands.CreateUser;

public record CreateUserCommand(string Name, string Email, string? Password = null);