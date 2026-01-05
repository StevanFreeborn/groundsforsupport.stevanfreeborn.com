using GroundsForSupport.Server.Payments;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GroundsForSupport.Server.Data;

internal sealed class Context(
  IOptions<ContextOptions> ctxOptions,
  DbContextOptions<Context> options
) : DbContext(options)
{
  private const string DataSourceKey = "Data Source=";
  private readonly ContextOptions _ctxOptions = ctxOptions.Value;

  public DbSet<Payment> Payments => Set<Payment>();

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    var dbPath = _ctxOptions.GetFullyQualifiedDatabasePath();
    var dbDirectory = Path.GetDirectoryName(dbPath) ?? throw new InvalidOperationException("Database directory path could not be determined.");

    if (Directory.Exists(dbDirectory) is false)
    {
      Directory.CreateDirectory(dbDirectory);
    }

    var connectionString = $"{DataSourceKey}{dbPath}";

    optionsBuilder.UseSqlite(connectionString);
  }
}