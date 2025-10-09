using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateCRMWinForms.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyIdToLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PropertyId",
                table: "Logs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Logs_PropertyId",
                table: "Logs",
                column: "PropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Properties_PropertyId",
                table: "Logs",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Properties_PropertyId",
                table: "Logs");

            migrationBuilder.DropIndex(
                name: "IX_Logs_PropertyId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "PropertyId",
                table: "Logs");
        }
    }
}