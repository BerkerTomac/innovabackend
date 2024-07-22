using MasrafTakip.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasrafTakip.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<ApplicationUserDto>> GetAllUsersAsync();
        Task<ApplicationUserDto> GetUserByIdAsync(string id);
        Task AddUserAsync(ApplicationUserDto userDto, string password);
        Task UpdateUserAsync(ApplicationUserDto userDto);
        Task DeleteUserAsync(string id);
    }
}
