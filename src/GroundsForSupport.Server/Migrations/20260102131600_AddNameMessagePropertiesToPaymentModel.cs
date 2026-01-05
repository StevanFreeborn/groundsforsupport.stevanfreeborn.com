using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroundsForSupport.Server.Migrations
{
  /// <inheritdoc />
  public partial class AddNameMessagePropertiesToPaymentModel : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<string>(
          name: "Message",
          table: "Payments",
          type: "TEXT",
          nullable: true);

      migrationBuilder.AddColumn<string>(
          name: "Name",
          table: "Payments",
          type: "TEXT",
          nullable: false,
          defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(
          name: "Message",
          table: "Payments");

      migrationBuilder.DropColumn(
          name: "Name",
          table: "Payments");
    }
  }
}