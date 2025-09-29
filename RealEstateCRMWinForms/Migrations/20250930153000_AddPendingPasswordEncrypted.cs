using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateCRMWinForms.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingPasswordEncrypted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PendingPasswordEncrypted",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PendingPasswordEncrypted",
                table: "Users");
        }
    }
}

