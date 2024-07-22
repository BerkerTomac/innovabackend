using MasrafTakip.Application.DTOs;
using MasrafTakip.Application.Interfaces;
using MasrafTakip.Domain.Entities;
using MasrafTakip.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            var users = await _userManager.Users.ToListAsync();
            return users.Select(u => new ApplicationUserDto
            {
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
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name
            };
        }

        public async Task<string> AddUserAsync(ApplicationUserDto userDto, string password)
        {
            var user = new ApplicationUser
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                Name = userDto.Name
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new ApplicationException("User creation failed! Please check user details and try again.");
            }

            return user.Id;
        }

        public async Task UpdateUserAsync(string id, ApplicationUserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.UserName = userDto.UserName;
                user.Email = userDto.Email;
                user.Name = userDto.Name;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    throw new ApplicationException("User update failed! Please check user details and try again.");
                }
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
