using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartSearch.Worker.FileMonitorService.Migrations
{
    /// <inheritdoc />
    public partial class AddedAppUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Department = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "SYSTEM"),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2023, 3, 11, 4, 2, 10, 373, DateTimeKind.Utc).AddTicks(7599)),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "SYSTEM"),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2023, 3, 11, 4, 2, 10, 373, DateTimeKind.Utc).AddTicks(7959))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUser", x => x.Id);
                });

            migrationBuilder.InsertData(
               table: "AppUser",
               columns: new[] { "Id", "FullName", "Email", "Password", "Department", "Role", "IsAdmin", "IsActive", "CreatedBy", "Created", "LastModifiedBy", "LastModified" },
               values: new object[] { 1, "Administrator", "admin@admin.org", "YWRtaW4=", 0, 1, true, true, "SYSTEM", DateTime.UtcNow, "SYSTEM", DateTime.UtcNow });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUser");
        }
    }
}
