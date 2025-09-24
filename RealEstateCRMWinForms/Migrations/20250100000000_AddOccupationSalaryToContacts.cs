using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateCRMWinForms.Migrations
{
    /// <inheritdoc />
    public partial class AddOccupationSalaryToContacts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if columns already exist before adding them
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Contacts' AND COLUMN_NAME = 'Occupation')
                BEGIN
                    ALTER TABLE Contacts ADD Occupation nvarchar(max) NOT NULL DEFAULT '';
                END
                
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Contacts' AND COLUMN_NAME = 'Salary')
                BEGIN
                    ALTER TABLE Contacts ADD Salary decimal(18,2) NULL;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Occupation",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Salary", 
                table: "Contacts");
        }
    }
}