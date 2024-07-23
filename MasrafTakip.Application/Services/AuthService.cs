using MasrafTakip.Application.DTOs;
using MasrafTakip.Application.Interfaces;
using MasrafTakip.Domain.Entities;
using MasrafTakip.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MasrafTakip.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<string> RegisterAsync(RegisterModel model)
        {
            try
            {

            
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                throw new Exception("User already exists!");

            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                Name = model.Name
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                throw new Exception("User creation failed! Please check user details and try again.");
            }

            return "User created successfully!";
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Registration");
                throw;
            }
        }

        public async Task<string> LoginAsync(LoginModel model)
        {
            try {

           
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var authClaims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.UserName), 
            new Claim("UserId", user.Id) // Custom claim for UserId
        };

                var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"])),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }


            }
            catch (Exception ex)
            {
                throw new Exception("Error in logging");
            }
            return null;
        }


    }
}
