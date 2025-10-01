using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateCRMWinForms.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSquareMetersField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SquareMeters",
                table: "Properties");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SquareMeters",
                table: "Properties",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
