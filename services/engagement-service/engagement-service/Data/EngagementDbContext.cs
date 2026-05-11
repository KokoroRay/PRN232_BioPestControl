using Microsoft.EntityFrameworkCore;
using engagement_service.Models;

namespace engagement_service.Data
{
    public class EngagementDbContext : DbContext
    {
        public EngagementDbContext(DbContextOptions<EngagementDbContext> options) : base(options)
        {
        }

        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<FeedbackImage> FeedbackImages { get; set; }
        public DbSet<FeedbackReply> FeedbackReplies { get; set; }
        public DbSet<FeedbackReviewTag> FeedbackReviewTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Feedback>(e =>
            {
                e.ToTable("Feedbacks");
                e.HasIndex(f => f.ProductId);
                e.HasIndex(f => f.CustomerId);
                e.HasIndex(f => new { f.OrderId, f.ProductId, f.CustomerId })
                    .IsUnique();
            });

            modelBuilder.Entity<FeedbackImage>(e =>
            {
                e.ToTable("FeedbackImages");
                e.HasOne(i => i.Feedback)
                    .WithMany(f => f.Images)
                    .HasForeignKey(i => i.FeedbackId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<FeedbackReply>(e =>
            {
                e.ToTable("FeedbackReplies");
                e.HasOne(r => r.Feedback)
                    .WithMany(f => f.Replies)
                    .HasForeignKey(r => r.FeedbackId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<FeedbackReviewTag>(e =>
            {
                e.ToTable("FeedbackReviewTags");
                e.HasKey(x => new { x.FeedbackId, x.ReviewTagId });
                e.HasOne(x => x.Feedback)
                    .WithMany(f => f.FeedbackTags)
                    .HasForeignKey(x => x.FeedbackId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
