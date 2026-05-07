using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using identity_service.Models;

namespace identity_service.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user)
        {
            // 1. Tạo khóa bảo mật (SymmetricKey) từ Secret Key trong file appsettings.json
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!));
            
            // 2. Chọn thuật toán mã hóa (HMAC SHA256)
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // 3. Khai báo các thông tin (Claims) sẽ được gói vào bên trong Token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),  // Subject (ID người dùng)
                new Claim(JwtRegisteredClaimNames.Email, user.Email),        // Email người dùng
                // Thêm cả hai dạng role claim:
                // - "role": chuẩn JWT, được JWT Bearer middleware đọc trực tiếp
                // - ClaimTypes.Role: chuẩn .NET, dùng cho User.IsInRole() / [Authorize(Roles)]
                new Claim("role", user.Role),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // ID duy nhất của token (chống replay attack)
            };

            // 4. Tạo ra JWT Token dựa trên các cấu hình ở trên
            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],     // Người phát hành Token
                audience: _config["JwtSettings:Audience"], // Nơi sẽ sử dụng Token
                claims: claims,                            // Dữ liệu đính kèm (Claims)
                expires: DateTime.Now.AddDays(7),          // Thời hạn sống của Token (ở đây là 7 ngày)
                signingCredentials: credentials);          // Chữ ký bảo mật

            // 5. Chuyển đổi đối tượng Token thành chuỗi văn bản (String) để trả về cho Client
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
