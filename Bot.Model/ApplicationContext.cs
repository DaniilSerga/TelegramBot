using Microsoft.EntityFrameworkCore;
using Bot.Model.DatabaseModels;

namespace Bot.Model
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Conversion> Conversions { get; set; } = null!;

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }
    }
}
