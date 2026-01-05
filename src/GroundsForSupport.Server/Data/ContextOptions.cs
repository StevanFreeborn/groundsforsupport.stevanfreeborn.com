using Microsoft.Extensions.Options;

namespace GroundsForSupport.Server.Data;

internal sealed record ContextOptions
{
  public string DatabaseFilePath { get; init; } = string.Empty;
  public string GetFullyQualifiedDatabasePath()
  {
    return Path.GetFullPath(DatabaseFilePath, AppContext.BaseDirectory);
  }
}

internal sealed record ContextOptionsSetup : IConfigureOptions<ContextOptions>
{
  private const string SectionName = nameof(ContextOptions);
  private readonly IConfiguration _configuration;

  public ContextOptionsSetup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public void Configure(ContextOptions options)
  {
    _configuration.GetSection(SectionName).Bind(options);
  }
}