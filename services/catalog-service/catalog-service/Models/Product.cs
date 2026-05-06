using BioPestControl.DAL.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioPestControl.DAL.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string SKU { get; set; } = null!; //MaSanPham - ma quan ly kho

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!; //TenSanPham - ten thuong mai

        [MaxLength(2000)]
        public string? Description { get; set; } //CongDung - cong dung, chi tieu va dieu tri

        [MaxLength(5000)]
        public string? UsageInstructions { get; set; } //HuongDanSD - huong dan su dung

        [MaxLength(400)]
        public string? Formulation { get; set; } = null!; //dang thuoc (han) - vi du: chat long, chay day, v.v.

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } //Gia

        public int StockQuantity { get; set; } //SoLuong

        public int? ExpirationDays { get; set; } //HanSd - han su dung tinh theo ngay

        public DateTime? ExpirationDate { get; set; } //ngay het han cu the (neu co)

        [MaxLength(100)]
        public string? UsageTarget { get; set; } //doi tuong su dung - vi du: cay trai, rau, lua, v.v.

        public ToxicityLevel ToxicityLevel { get; set; } //muc do doc hai - GHS classification

        public int PreHarvestInterval { get; set; } //thoi gian cach ly truoc thu hoach (ngay) - safety interval

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DoseRate { get; set; } //lieu dung khuyy ngh - g/l hoac ml/l

        [MaxLength(500)]
        public string? SafetyInformation { get; set; } //thong tin an toan - cang bao ghi nhan

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        public bool IsActive { get; set; } = true; //IsActive - trang thai san pham

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public int CategoryId { get; set; } //MaNhomSP
        public int? CreatedByAdminId { get; set; }
        public int? ManagedByStaffId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; } = null!;
        
        public virtual Admin? CreatedByAdmin { get; set; }
        public virtual Staff? ManagedByStaff { get; set; }

        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public virtual ICollection<ProductChemical> ProductChemicals { get; set; } = new List<ProductChemical>();
        public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
        public virtual ICollection<WarehouseLog> WarehouseLogs { get; set; } = new List<WarehouseLog>();
    }
}
