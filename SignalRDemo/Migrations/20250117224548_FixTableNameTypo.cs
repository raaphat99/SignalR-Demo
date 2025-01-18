using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SignalRDemo.Migrations
{
    /// <inheritdoc />
    public partial class FixTableNameTypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserConnection_Users_UserId",
                table: "UserConnection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserConnection",
                table: "UserConnection");

            migrationBuilder.RenameTable(
                name: "UserConnection",
                newName: "UserConnections");

            migrationBuilder.RenameIndex(
                name: "IX_UserConnection_UserId",
                table: "UserConnections",
                newName: "IX_UserConnections_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserConnections",
                table: "UserConnections",
                column: "ConnectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserConnections_Users_UserId",
                table: "UserConnections",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserConnections_Users_UserId",
                table: "UserConnections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserConnections",
                table: "UserConnections");

            migrationBuilder.RenameTable(
                name: "UserConnections",
                newName: "UserConnection");

            migrationBuilder.RenameIndex(
                name: "IX_UserConnections_UserId",
                table: "UserConnection",
                newName: "IX_UserConnection_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserConnection",
                table: "UserConnection",
                column: "ConnectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserConnection_Users_UserId",
                table: "UserConnection",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
