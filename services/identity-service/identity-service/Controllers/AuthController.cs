using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Google.Apis.Auth;
using identity_service.Data;
using identity_service.Models;
using identity_service.DTOs;
using identity_service.Services;

namespace identity_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtTokenService _jwtTokenService;
        private readonly IConfiguration _configuration;

        // Khai báo Constructor sử dụng Dependency Injection để lấy các Services
        public AuthController(AppDbContext context, JwtTokenService jwtTokenService, IConfiguration configuration)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
            _configuration = configuration;
        }

        // [POST] api/auth/register - API Đăng ký tài khoản
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Kiểm tra xem email đã tồn tại trong database chưa
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (existingUser != null)
            {
                // Điểm bổ sung 3: Nếu tài khoản đã tồn tại nhưng chưa có mật khẩu
                // (tức là trước đó user này đã đăng nhập bằng Google), cho phép cập nhật mật khẩu
                if (string.IsNullOrEmpty(existingUser.PasswordHash))
                {
                    existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                    if (!string.IsNullOrEmpty(request.FullName))
                    {
                        existingUser.FullName = request.FullName;
                    }
                    existingUser.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return Ok(new { Message = "Đã cập nhật mật khẩu thành công cho tài khoản của bạn." });
                }

                return BadRequest(new { Message = "Email đã tồn tại." });
            }

            // Tạo thông tin người dùng mới
            var user = new User
            {
                Email = request.Email,
                FullName = request.FullName,
                // Mã hóa (Hash) mật khẩu bằng thuật toán BCrypt trước khi lưu vào database để bảo mật
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            // Thêm vào database và lưu lại
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Đăng ký thành công" });
        }

        // [POST] api/auth/login - API Đăng nhập truyền thống
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Tìm người dùng theo Email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            
            // Nếu không tìm thấy user, hoặc user này (chỉ đăng nhập bằng google) không có mật khẩu (PasswordHash)
            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            {
                return Unauthorized(new { Message = "Email hoặc mật khẩu không chính xác." });
            }

            // Điểm bổ sung 1: Kiểm tra tài khoản có bị khóa không
            if (!user.IsActive)
            {
                return BadRequest(new { Message = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ Admin." });
            }

            // Kiểm tra xem mật khẩu người dùng nhập có khớp với mã Hash trong CSDL không
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { Message = "Email hoặc mật khẩu không chính xác." });
            }

            // Mật khẩu đúng, tiến hành tạo JWT Token
            var token = _jwtTokenService.GenerateToken(user);

            // Trả về thông tin và Token cho Client
            return Ok(new AuthResponse
            {
                Token = token,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                AvatarUrl = user.AvatarUrl
            });
        }

        // [POST] api/auth/google-login - API Đăng nhập bằng Google
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            GoogleJsonWebSignature.Payload payload;
            try
            {
                // Điểm bổ sung 2: Xác thực token kèm theo ClientId để đảm bảo token này
                // được cấp đúng cho ứng dụng của mình, tránh token từ ứng dụng khác bắn vào
                var validationSettings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { _configuration["GoogleOptions:ClientId"]! }
                };
                payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, validationSettings);
            }
            catch (InvalidJwtException)
            {
                return BadRequest(new { Message = "Token Google không hợp lệ." });
            }

            // Tìm xem email này đã đăng ký trong hệ thống chưa
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);
            
            if (user == null)
            {
                // Nếu User chưa tồn tại trong hệ thống, tiến hành tạo mới tự động (Đăng ký nhanh)
                user = new User
                {
                    Email = payload.Email,
                    FullName = payload.Name,
                    GoogleId = payload.Subject, // Subject ID (ID định danh) của người dùng trên hệ thống Google
                    AvatarUrl = payload.Picture, // Lấy ảnh đại diện từ Google
                    Role = "Customer"
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            else if (string.IsNullOrEmpty(user.GoogleId))
            {
                // Link account: Nếu người dùng trước đó đã đăng ký bằng form (Email/Pass)
                // bây giờ họ lại bấm Đăng nhập bằng Google, ta sẽ cập nhật thêm GoogleId cho họ.
                user.GoogleId = payload.Subject;
                if (string.IsNullOrEmpty(user.FullName)) 
                {
                    // Cập nhật tên nếu trước đó họ chưa nhập
                    user.FullName = payload.Name;
                }
                if (string.IsNullOrEmpty(user.AvatarUrl))
                {
                    user.AvatarUrl = payload.Picture;
                }
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            // Điểm bổ sung 1: Kiểm tra tài khoản có bị khóa không (dù đăng nhập qua Google)
            if (!user.IsActive)
            {
                return BadRequest(new { Message = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ Admin." });
            }

            // Tạo JWT Token nội bộ của hệ thống sau khi đã đăng nhập bằng Google hợp lệ
            var token = _jwtTokenService.GenerateToken(user);

            return Ok(new AuthResponse
            {
                Token = token,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                AvatarUrl = user.AvatarUrl
            });
        }
    }
}
