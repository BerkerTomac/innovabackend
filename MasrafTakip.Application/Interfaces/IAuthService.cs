using System.Threading.Tasks;
using MasrafTakip.Application.DTOs;

namespace MasrafTakip.Application.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterModel model);
        Task<string> LoginAsync(LoginModel model);
    }
}
