using Backend.Dto;
using Backend.Models;

namespace Backend.Interfaces
{
    public interface IUserRepository
    {
        Task<User> Authenticate(string usernameOrEmail, string password);
        void Register(UserDto user1);
        Task<bool> UserAlreadyExists(string username, string email);
        Task<bool> UserAlreadyExistsByUsername(string userName);
        Task<bool> UserAlreadyExistsByEmail(string email);

    }
}
