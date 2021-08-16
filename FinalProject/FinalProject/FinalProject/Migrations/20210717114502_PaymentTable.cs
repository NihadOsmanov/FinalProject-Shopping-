using Microsoft.EntityFrameworkCore.Migrations;

namespace FinalProject.Migrations
{
    public partial class PaymentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisaImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MastercardImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaypalImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmericanExpressImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscoverImage = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");
        }
    }
}
