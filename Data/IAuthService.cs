using AppointmentScheduling.DTOs;

namespace AppointmentScheduling.Data
{
    public interface IAuthService
    {
        Task<TokenResponseDto> LoginAsync(LoginDto loginDto);
        Task<bool> RegisterAsync(RegisterDto registerDto);
        Task<TokenResponseDto> RefreshTokenAsync(string refreshToken);
    }
}
