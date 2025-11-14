using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMembershipImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MembershipTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "/Gym_Tem/img/memberships/adult.jpg");

            migrationBuilder.UpdateData(
                table: "MembershipTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "/Gym_Tem/img/memberships/senior.jpg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MembershipTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "MembershipTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: null);
        }
    }
}
