using MasrafTakip.Application.DTOs;
using MasrafTakip.Application.Interfaces;
using MasrafTakip.Domain.Entities;
using MasrafTakip.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasrafTakip.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<ApplicationUserDto>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            return users.Select(u => new ApplicationUserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Name = u.Name
            }).ToList();
        }

        public async Task<ApplicationUserDto> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return null;

            return new ApplicationUserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name
            };
        }

        public async Task AddUserAsync(ApplicationUserDto userDto, string password)
        {
            var user = new ApplicationUser
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                Name = userDto.Name
            };
            await _userManager.CreateAsync(user, password);
        }

        public async Task UpdateUserAsync(ApplicationUserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id);
            if (user != null)
            {
                user.UserName = userDto.UserName;
                user.Email = userDto.Email;
                user.Name = userDto.Name;
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }
    }
}
