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

            // This down here is a composite primary key for UserGroup
            modelBuilder.Entity<UserGroup>()
                .HasKey(ug => new { ug.UserId, ug.GroupId });
        }
    }
}
