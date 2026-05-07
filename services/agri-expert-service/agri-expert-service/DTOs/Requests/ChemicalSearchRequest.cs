namespace agri_expert_service.DTOs.Requests
{
    public class ChemicalSearchRequest
    {
        // Tìm theo tên, tên tiếng Việt, CAS number, nhóm hóa chất hoặc mô tả
        public string? Keyword { get; set; }

        // Lọc theo nhóm hóa chất (exact match)
        public string? ChemicalGroup { get; set; }

        // Lọc theo mức độ độc hại (Ia, Ib, II, III, U)
        public string? ToxicityLevel { get; set; }

        // Lọc theo trạng thái (null = tất cả)
        public bool? IsActive { get; set; }

        // Phân trang
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Sắp xếp: "name" | "group" | "toxicity" | "createdAt"
        public string SortBy { get; set; } = "name";
        public bool SortDesc { get; set; } = false;
    }
}
