using Microsoft.EntityFrameworkCore;

namespace FSSA.Models
{
    public class ProjectManagerContext : DbContext
    {
        public ProjectManagerContext(DbContextOptions<ProjectManagerContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ProjectLevel> ProjectLevels { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<ProposalLog> ProposalLogs { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<EthicsReview> EthicsReviews { get; set; }
        public DbSet<AttachmentType> AttachmentTypes { get; set; }
        public DbSet<ProposalResearcher> ProposalResearchers { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<ProposalHistory> ProposalHistories { get; set; }
        public DbSet<FinancialResource> FinancialResources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Group>().ToTable("groups");
            modelBuilder.Entity<Role>().ToTable("roles");
            modelBuilder.Entity<UserGroup>().ToTable("user_groups");
            modelBuilder.Entity<Notification>().ToTable("notifications");
            modelBuilder.Entity<ProjectLevel>().ToTable("project_levels");
            modelBuilder.Entity<Status>().ToTable("status");
            modelBuilder.Entity<Proposal>().ToTable("proposals");
            modelBuilder.Entity<ProposalLog>().ToTable("proposal_log");
            modelBuilder.Entity<Attachment>().ToTable("attachments");
            modelBuilder.Entity<Review>().ToTable("reviews");
            modelBuilder.Entity<Comment>().ToTable("comments");
            modelBuilder.Entity<EthicsReview>().ToTable("ethics_review");
            modelBuilder.Entity<UserRole>().ToTable("user_roles");
            modelBuilder.Entity<AttachmentType>().ToTable("attachment_types");
            modelBuilder.Entity<FinancialResource>().ToTable("financial_resources");
            modelBuilder.Entity<ProposalResearcher>().ToTable("proposal_researchers");
            
            // This down here is a composite primary key for UserGroup
            modelBuilder.Entity<UserGroup>()
                .HasKey(ug => new { ug.UserId, ug.GroupId });

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new {ur.UserId, ur.RoleId});

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);
            
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany()
                .HasForeignKey(ur => ur.RoleId);
            
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Proposal)
                .WithMany(p => p.Notifications)
                .HasForeignKey(n => n.ProposalId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
            
            modelBuilder.Entity<Attachment>()
                .HasOne(a => a.AttachmentType)
                .WithMany()
                .HasForeignKey(a => a.TypeId);

            modelBuilder.Entity<ProposalResearcher>()
            .HasKey(pr => new { pr.ProposalId, pr.UserId });

            modelBuilder.Entity<ProposalResearcher>()
                .HasOne(pr => pr.Proposal)
                .WithMany()
                .HasForeignKey(pr => pr.ProposalId);

            modelBuilder.Entity<ProposalResearcher>()
                .HasOne(pr => pr.User)
                .WithMany()
                .HasForeignKey(pr => pr.UserId);
        }
    }
}
