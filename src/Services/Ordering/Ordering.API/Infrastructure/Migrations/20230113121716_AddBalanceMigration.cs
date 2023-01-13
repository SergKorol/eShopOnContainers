using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordering.API.Infrastructure.Migrations
{
    public partial class AddBalanceMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                schema: "ordering",
                table: "orders",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                schema: "ordering",
                table: "orders");
        }
    }
}
