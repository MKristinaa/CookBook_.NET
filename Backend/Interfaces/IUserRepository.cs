using Backend.Dto;
using Backend.Models;

namespace Backend.Interfaces
{
    public interface IUserRepository
    {
        Task<User> Authenticate(string username, string password);
        void Register(UserDto user1);
        Task<bool> UserAlreadyExists(string username);

    }
}
