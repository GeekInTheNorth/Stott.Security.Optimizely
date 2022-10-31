namespace Stott.Security.Optimizely.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class CspDataContextFactory : IDesignTimeDbContextFactory<CspDataContext>
{
    public CspDataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CspDataContext>();
        optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=EPiServerDB_565f2030;User Id=EPiServerDB_0aa972a3User;Password=R4&leNE*JIjF4bFA@3d^O42hN;MultipleActiveResultSets=True");

        return new CspDataContext(optionsBuilder.Options);
    }
}