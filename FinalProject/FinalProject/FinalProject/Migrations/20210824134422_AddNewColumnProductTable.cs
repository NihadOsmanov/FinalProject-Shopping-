using Microsoft.EntityFrameworkCore.Migrations;

namespace FinalProject.Migrations
{
    public partial class AddNewColumnProductTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewPice",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "OldPrice",
                table: "Products",
                newName: "Price");

            migrationBuilder.AddColumn<int>(
                name: "Discount",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Products",
                newName: "OldPrice");

            migrationBuilder.AddColumn<string>(
                name: "NewPice",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
