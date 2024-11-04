using Api.B2B.Core.Dtos;
using System.Threading.Tasks;

namespace Api.B2B.Core.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task LogoutAsync(string token);
        Task<bool> IsTokenBlacklisted(string token);
    }
}
