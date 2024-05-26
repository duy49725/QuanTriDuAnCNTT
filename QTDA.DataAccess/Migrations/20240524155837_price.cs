using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QTDA.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class price : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListPrice",
                table: "products");

            migrationBuilder.DropColumn(
                name: "Price100",
                table: "products");

            migrationBuilder.DropColumn(
                name: "Price50",
                table: "products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ListPrice",
                table: "products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Price100",
                table: "products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Price50",
                table: "products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
