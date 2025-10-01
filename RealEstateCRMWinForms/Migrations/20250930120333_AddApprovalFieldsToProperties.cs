using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateCRMWinForms.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalFieldsToProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Properties",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SubmittedByUserId",
                table: "Properties",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "SubmittedByUserId",
                table: "Properties");
        }
    }
}
