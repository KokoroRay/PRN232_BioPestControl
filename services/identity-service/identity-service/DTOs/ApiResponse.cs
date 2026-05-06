namespace identity_service.DTOs
{
    // Wrapper chuẩn hóa toàn bộ response trả về cho Client
    // T là kiểu dữ liệu của phần Data (có thể là AuthResponse, UserDto, List<UserDto>, ...)
    public class ApiResponse<T>
    {
        // Cho biết request có thành công không
        public bool Success { get; set; }

        // Thông điệp mô tả kết quả (ví dụ: "Đăng nhập thành công", "Email đã tồn tại")
        public string? Message { get; set; }

        // Dữ liệu trả về (null nếu thất bại hoặc không có data)
        public T? Data { get; set; }
    }
}
