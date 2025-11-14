using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddMembershipImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MembershipTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "/Gym_Tem/img/memberships/student.jpg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MembershipTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: null);
        }
    }
}
