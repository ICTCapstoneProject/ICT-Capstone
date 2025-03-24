using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace YourNamespace.Data
{
    public class ProjectManagerContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public ProjectManagerContext(DbContextOptions<ProjectManagerContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
