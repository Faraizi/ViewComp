using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankCoreCRUD.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Account_AccID",
                table: "Customer");

            migrationBuilder.RenameColumn(
                name: "AccID",
                table: "Customer",
                newName: "AcctID");

            migrationBuilder.RenameIndex(
                name: "IX_Customer_AccID",
                table: "Customer",
                newName: "IX_Customer_AcctID");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Account_AcctID",
                table: "Customer",
                column: "AcctID",
                principalTable: "Account",
                principalColumn: "AcctID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Account_AcctID",
                table: "Customer");

            migrationBuilder.RenameColumn(
                name: "AcctID",
                table: "Customer",
                newName: "AccID");

            migrationBuilder.RenameIndex(
                name: "IX_Customer_AcctID",
                table: "Customer",
                newName: "IX_Customer_AccID");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Account_AccID",
                table: "Customer",
                column: "AccID",
                principalTable: "Account",
                principalColumn: "AcctID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
