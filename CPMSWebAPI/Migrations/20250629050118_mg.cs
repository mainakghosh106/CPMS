using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CPMSWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class mg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "Role",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Role", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "userHierarchyHistory",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        SupervisorId = table.Column<int>(type: "int", nullable: false),
            //        SubordinateId = table.Column<int>(type: "int", nullable: false),
            //        DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_userHierarchyHistory", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "users",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        RoleId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_users", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_users_Role_RoleId",
            //            column: x => x.RoleId,
            //            principalTable: "Role",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Project",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        ActualStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        ActualEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CreatedByUserId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Project", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Project_users_CreatedByUserId",
            //            column: x => x.CreatedByUserId,
            //            principalTable: "users",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UserHierarchy",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        SupervisorId = table.Column<int>(type: "int", nullable: false),
            //        SubordinateId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserHierarchy", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_UserHierarchy_users_SubordinateId",
            //            column: x => x.SubordinateId,
            //            principalTable: "users",
            //            principalColumn: "Id");
            //        table.ForeignKey(
            //            name: "FK_UserHierarchy_users_SupervisorId",
            //            column: x => x.SupervisorId,
            //            principalTable: "users",
            //            principalColumn: "Id");
            //    });

            migrationBuilder.CreateTable(
                name: "ProjectProgressLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttachmentPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoursSpent = table.Column<float>(type: "real", nullable: true),
                    LogDoneBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectProgressLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectProgressLog_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            //migrationBuilder.CreateTable(
            //    name: "ProjectUserMapping",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ProjectId = table.Column<int>(type: "int", nullable: false),
            //        UserId = table.Column<int>(type: "int", nullable: false),
            //        AssignedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ProjectUserMapping", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_ProjectUserMapping_Project_ProjectId",
            //            column: x => x.ProjectId,
            //            principalTable: "Project",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_ProjectUserMapping_users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "users",
            //            principalColumn: "Id");
            //    });

            //migrationBuilder.InsertData(
            //    table: "Role",
            //    columns: new[] { "Id", "RoleName" },
            //    values: new object[,]
            //    {
            //        { 1, "Admin" },
            //        { 2, "Manager" },
            //        { 3, "TeamLead" }
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Project_CreatedByUserId",
            //    table: "Project",
            //    column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProgressLog_ProjectId",
                table: "ProjectProgressLog",
                column: "ProjectId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ProjectUserMapping_ProjectId",
            //    table: "ProjectUserMapping",
            //    column: "ProjectId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ProjectUserMapping_UserId",
            //    table: "ProjectUserMapping",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserHierarchy_SubordinateId",
            //    table: "UserHierarchy",
            //    column: "SubordinateId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserHierarchy_SupervisorId",
            //    table: "UserHierarchy",
            //    column: "SupervisorId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_users_RoleId",
            //    table: "users",
            //    column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectProgressLog");

            migrationBuilder.DropTable(
                name: "ProjectUserMapping");

            migrationBuilder.DropTable(
                name: "UserHierarchy");

            migrationBuilder.DropTable(
                name: "userHierarchyHistory");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
