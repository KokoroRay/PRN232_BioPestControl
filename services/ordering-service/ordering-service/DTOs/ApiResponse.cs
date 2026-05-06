namespace ordering_service.DTOs
{
    // Wrapper chuẩn hóa toàn bộ response của ordering-service
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}
