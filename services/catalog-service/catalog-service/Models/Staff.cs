using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BioPestControl.DAL.Entities
{
    public class Staff
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Email { get; set; } = null!;
        [Required]
        [MaxLength(200)]
        public string PasswordHash { get; set; } = null!;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Role { get; set; }

        [MaxLength(500)]
        public string? AvatarUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        public int? CreatedByAdminId { get; set; }
        public virtual Admin? CreatedByAdmin { get; set; }

        // Navigation Properties
        public virtual ICollection<FeedbackReply> FeedbackReplies { get; set; } = new List<FeedbackReply>();
        public virtual ICollection<Customer> ManagedCustomers { get; set; } = new List<Customer>();
        public virtual ICollection<Order> ManagedOrders { get; set; } = new List<Order>();
        public virtual ICollection<Article> ManagedArticles { get; set; } = new List<Article>();
        public virtual ICollection<Product> ManagedProducts { get; set; } = new List<Product>();
        public virtual ICollection<Category> ManagedCategories { get; set; } = new List<Category>();
        public virtual ICollection<Warehouse> ManagedWarehouses { get; set; } = new List<Warehouse>();
        public virtual ICollection<WarehouseLog> WarehouseLogs { get; set; } = new List<WarehouseLog>();
        public virtual ICollection<ChemicalProfile> ManagedChemicalProfiles { get; set; } = new List<ChemicalProfile>();
    }
}
