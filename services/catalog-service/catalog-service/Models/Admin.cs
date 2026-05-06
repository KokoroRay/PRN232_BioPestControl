using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioPestControl.DAL.Entities
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string PasswordHash { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = null!;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(500)]
        public string? AvatarUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        public virtual ICollection<Staff> ManagedStaffs { get; set; } = new List<Staff>();
        public virtual ICollection<Category> ManagedCategories { get; set; } = new List<Category>();
        public virtual ICollection<Product> ManagedProducts { get; set; } = new List<Product>();
        public virtual ICollection<Discount> ManagedDiscounts { get; set; } = new List<Discount>();
        public virtual ICollection<ChemicalProfile> ManagedChemicalProfiles { get; set; } = new List<ChemicalProfile>();
        public virtual ICollection<Warehouse> ManagedWarehouses { get; set; } = new List<Warehouse>();
        public virtual ICollection<WarehouseLog> WarehouseLogs { get; set; } = new List<WarehouseLog>();
        public virtual ICollection<Article> ManagedArticles { get; set; } = new List<Article>();
    }
}
