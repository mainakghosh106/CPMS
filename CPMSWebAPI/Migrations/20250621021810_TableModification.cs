using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPMSWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class TableModification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_userRole_Role_RoleId",
                table: "userRole");

            migrationBuilder.DropForeignKey(
                name: "FK_userRole_users_UserId",
                table: "userRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_userRole",
                table: "userRole");

            migrationBuilder.DropIndex(
                name: "IX_userRole_UserId",
                table: "userRole");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "userRole");

            migrationBuilder.RenameTable(
                name: "userRole",
                newName: "UserRole");

            migrationBuilder.RenameIndex(
                name: "IX_userRole_RoleId",
                table: "UserRole",
                newName: "IX_UserRole_RoleId");

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_users_RoleId",
                table: "users",
                column: "RoleId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_Role_RoleId",
                table: "UserRole",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_UserRole_RoleId",
                table: "users",
                column: "RoleId",
                principalTable: "UserRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_Role_RoleId",
                table: "UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_users_UserRole_RoleId",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_RoleId",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "users");

            migrationBuilder.RenameTable(
                name: "UserRole",
                newName: "userRole");

            migrationBuilder.RenameIndex(
                name: "IX_UserRole_RoleId",
                table: "userRole",
                newName: "IX_userRole_RoleId");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "userRole",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_userRole",
                table: "userRole",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_userRole_UserId",
                table: "userRole",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_userRole_Role_RoleId",
                table: "userRole",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_userRole_users_UserId",
                table: "userRole",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
