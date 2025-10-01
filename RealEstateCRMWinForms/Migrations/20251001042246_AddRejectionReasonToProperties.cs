using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateCRMWinForms.Migrations
{
    /// <inheritdoc />
    public partial class AddRejectionReasonToProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Properties");
        }
    }
}
