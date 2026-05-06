using System.ComponentModel.DataAnnotations;

namespace inventory_service.DTOs.Requests
{
    /// <summary>
    /// DTO đại diện cho một dòng sản phẩm trong yêu cầu nhập kho
    /// </summary>
    public class ImportProductItem
    {
        [Required(ErrorMessage = "Mã SKU sản phẩm là bắt buộc")]
        public string SKU { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng nhập phải lớn hơn 0")]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá nhập không hợp lệ")]
        public decimal ImportPrice { get; set; }
        
        public DateTime? ExpirationDate { get; set; }
    }

    /// <summary>
    /// DTO yêu cầu nhập kho nhiều sản phẩm cùng lúc
    /// </summary>
    public class ImportProductsRequest
    {
        [Required(ErrorMessage = "Danh sách sản phẩm nhập không được để trống")]
        [MinLength(1, ErrorMessage = "Phải nhập ít nhất 1 sản phẩm")]
        public List<ImportProductItem> Items { get; set; } = new();

        public string? SupplierName { get; set; }

        public string? Note { get; set; }
    }
}
