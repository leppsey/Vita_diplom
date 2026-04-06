using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Isomerization.Domain.Data;

public class IsomerizationContextFactory : IDesignTimeDbContextFactory<IsomerizationContext>
{
    public IsomerizationContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IsomerizationContext>();
        optionsBuilder.UseSqlite($"Data Source={IsomerizationContext.DbPath}");
        return new IsomerizationContext(optionsBuilder.Options);
    }
}
