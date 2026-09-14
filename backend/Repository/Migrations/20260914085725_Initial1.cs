using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class Initial1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppSessions_DeviceId",
                table: "AppSessions");

            migrationBuilder.CreateIndex(
                name: "IX_AppSessions_DeviceId_StartTime",
                table: "AppSessions",
                columns: new[] { "DeviceId", "StartTime" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppSessions_DeviceId_StartTime",
                table: "AppSessions");

            migrationBuilder.CreateIndex(
                name: "IX_AppSessions_DeviceId",
                table: "AppSessions",
                column: "DeviceId");
        }
    }
}
