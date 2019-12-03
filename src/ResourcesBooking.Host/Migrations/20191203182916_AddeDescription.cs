using Microsoft.EntityFrameworkCore.Migrations;

namespace ResourcesBooking.Host.Migrations
{
    public partial class AddeDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Resources",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Resources");
        }
    }
}
