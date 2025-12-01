using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MembershipPurchases_AspNetUsers_UserId",
                table: "MembershipPurchases");

            migrationBuilder.DropIndex(
                name: "IX_MembershipPurchases_UserId",
                table: "MembershipPurchases");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MembershipPurchases");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "MembershipPurchases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "MembershipPurchases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "MembershipPurchases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "MembershipPurchases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "MembershipPurchases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "MembershipPurchases");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "MembershipPurchases");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "MembershipPurchases");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "MembershipPurchases");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "MembershipPurchases");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "MembershipPurchases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MembershipPurchases_UserId",
                table: "MembershipPurchases",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MembershipPurchases_AspNetUsers_UserId",
                table: "MembershipPurchases",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
