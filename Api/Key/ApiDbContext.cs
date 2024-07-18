using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Api.Key
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        { }

        public DbSet<ApiKey> ApiKeys => Set<ApiKey>();
    }
}
