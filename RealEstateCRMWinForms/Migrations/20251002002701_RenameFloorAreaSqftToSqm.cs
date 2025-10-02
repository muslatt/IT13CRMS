using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateCRMWinForms.Migrations
{
    /// <inheritdoc />
    public partial class RenameFloorAreaSqftToSqm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, add the new column
            migrationBuilder.AddColumn<decimal>(
                name: "FloorAreaSqm",
                table: "Properties",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            // Convert existing data from sqft to sqm (1 sqft = 0.092903 sqm)
            migrationBuilder.Sql(@"
                UPDATE Properties 
                SET FloorAreaSqm = FloorAreaSqft * 0.092903
                WHERE FloorAreaSqft IS NOT NULL
            ");

            // Drop the old column
            migrationBuilder.DropColumn(
                name: "FloorAreaSqft",
                table: "Properties");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add back the old column
            migrationBuilder.AddColumn<decimal>(
                name: "FloorAreaSqft",
                table: "Properties",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            // Convert data back from sqm to sqft (1 sqm = 10.7639 sqft)
            migrationBuilder.Sql(@"
                UPDATE Properties 
                SET FloorAreaSqft = FloorAreaSqm * 10.7639
                WHERE FloorAreaSqm IS NOT NULL
            ");

            // Drop the new column
            migrationBuilder.DropColumn(
                name: "FloorAreaSqm",
                table: "Properties");
        }
    }
}
