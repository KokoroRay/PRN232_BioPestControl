using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace catalog_service.Models
{
    public class ProductCrop
    {
        public int ProductId { get; set; }
        
        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; } = null!;

        public int CropId { get; set; }
        
        [ForeignKey(nameof(CropId))]
        public virtual Crop Crop { get; set; } = null!;

        [MaxLength(500)]
        public string? UsageInstruction { get; set; } // Hướng dẫn sử dụng hoặc lý do phù hợp (VD: Trừ bệnh đạo ôn trên lúa)
    }
}
