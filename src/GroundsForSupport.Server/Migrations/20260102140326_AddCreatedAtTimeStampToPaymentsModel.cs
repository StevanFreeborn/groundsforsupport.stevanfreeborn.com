using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroundsForSupport.Server.Migrations
{
  /// <inheritdoc />
  public partial class AddCreatedAtTimeStampToPaymentsModel : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<long>(
          name: "CreatedAtUnix",
          table: "Payments",
          type: "INTEGER",
          nullable: false,
          defaultValue: 0L);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(
          name: "CreatedAtUnix",
          table: "Payments");
    }
  }
}