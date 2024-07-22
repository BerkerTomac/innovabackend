using MasrafTakip.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasrafTakip.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<ApplicationUserDto>> GetAllUsersAsync();
        Task<ApplicationUserDto> GetUserByIdAsync(string id);
        Task<string> AddUserAsync(ApplicationUserDto userDto, string password);
        Task UpdateUserAsync(string id, ApplicationUserDto userDto);
        Task DeleteUserAsync(string id);
    }
}
