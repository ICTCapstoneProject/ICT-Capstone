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
    }
}
