using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateCRMWinForms.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAgentColumnFromProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Agent",
                table: "Properties");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Agent",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
