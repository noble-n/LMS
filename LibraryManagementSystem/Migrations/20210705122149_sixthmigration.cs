using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class sixthmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CheckOutRef",
                table: "CheckOut",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckOutRef",
                table: "Book",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckOutRef",
                table: "CheckOut");

            migrationBuilder.DropColumn(
                name: "CheckOutRef",
                table: "Book");
        }
    }
}
