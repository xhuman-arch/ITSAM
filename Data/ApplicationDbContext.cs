using ITServiceAssetManagement.Models;
using ITServiceAssetManagement.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ITServiceAssetManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketAssignment> TicketAssignments { get; set; }
        public DbSet<Troubleshooting> Troubleshootings { get; set; }
        public DbSet<ReplacementRequest> ReplacementRequests { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Attachment> Attachments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Ticket - Requester
            builder.Entity<Ticket>()
                .HasOne(t => t.Requester)
                .WithMany()
                .HasForeignKey(t => t.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ticket - Asset
            builder.Entity<Ticket>()
                .HasOne(t => t.Asset)
                .WithMany(a => a.Tickets)
                .HasForeignKey(t => t.AssetId)
                .OnDelete(DeleteBehavior.SetNull);

            // TicketAssignment - Ticket
            builder.Entity<TicketAssignment>()
                .HasOne(ta => ta.Ticket)
                .WithMany(t => t.TicketAssignments)
                .HasForeignKey(ta => ta.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // TicketAssignment - AssignedTo
            builder.Entity<TicketAssignment>()
                .HasOne(ta => ta.AssignedTo)
                .WithMany()
                .HasForeignKey(ta => ta.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            // Troubleshooting - Ticket
            builder.Entity<Troubleshooting>()
                .HasOne(tr => tr.Ticket)
                .WithMany(t => t.Troubleshootings)
                .HasForeignKey(tr => tr.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // Troubleshooting - PerformedBy
            builder.Entity<Troubleshooting>()
                .HasOne(tr => tr.PerformedBy)
                .WithMany()
                .HasForeignKey(tr => tr.PerformedById)
                .OnDelete(DeleteBehavior.Restrict);

            // ReplacementRequest - Ticket
            builder.Entity<ReplacementRequest>()
                .HasOne(r => r.Ticket)
                .WithMany(t => t.ReplacementRequests)
                .HasForeignKey(r => r.TicketId)
                .OnDelete(DeleteBehavior.SetNull);

            // ReplacementRequest - Asset
            builder.Entity<ReplacementRequest>()
                .HasOne(r => r.Asset)
                .WithMany(a => a.ReplacementRequests)
                .HasForeignKey(r => r.AssetId)
                .OnDelete(DeleteBehavior.SetNull);

            // ReplacementRequest - RequestedBy
            builder.Entity<ReplacementRequest>()
                .HasOne(r => r.RequestedBy)
                .WithMany()
                .HasForeignKey(r => r.RequestedById)
                .OnDelete(DeleteBehavior.Restrict);

            // ReplacementRequest - ProcessedBy
            builder.Entity<ReplacementRequest>()
                .HasOne(r => r.ProcessedBy)
                .WithMany()
                .HasForeignKey(r => r.ProcessedById)
                .OnDelete(DeleteBehavior.Restrict);

            // AuditLog - PerformedBy
            builder.Entity<AuditLog>()
                .HasOne(a => a.PerformedBy)
                .WithMany()
                .HasForeignKey(a => a.PerformedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Notification - User
            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Attachment - Ticket
            builder.Entity<Attachment>()
                .HasOne(a => a.Ticket)
                .WithMany(t => t.Attachments)
                .HasForeignKey(a => a.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // Attachment - Troubleshooting
            builder.Entity<Attachment>()
                .HasOne(a => a.Troubleshooting)
                .WithMany(t => t.Attachments)
                .HasForeignKey(a => a.TroubleshootingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Attachment - UploadedBy
            builder.Entity<Attachment>()
                .HasOne(a => a.UploadedBy)
                .WithMany()
                .HasForeignKey(a => a.UploadedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
