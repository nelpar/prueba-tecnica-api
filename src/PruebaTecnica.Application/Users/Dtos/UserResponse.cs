
using PruebaTecnica.Domain.Entities;

namespace PruebaTecnica.Application.Users.Dtos;

public record UserResponse(int Id, string Name, string Email, bool IsActive, DateTime CreatedAt)
{
    public static UserResponse FromEntity(User user) =>
        new(user.Id, user.Name, user.Email, user.IsActive, user.CreatedAt);
}