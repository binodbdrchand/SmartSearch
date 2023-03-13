using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartSearch.Worker.FileMonitorService.Migrations
{
    /// <inheritdoc />
    public partial class AddedVideoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Video",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Language = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    TopicProbabilty = table.Column<decimal>(type: "decimal(20,18)", precision: 20, scale: 18, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Video", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VideoTopic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(type: "int", nullable: false),
                    VideoId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoTopic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoTopic_Video_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Video",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoKeyword",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsInClipText = table.Column<bool>(type: "bit", nullable: false),
                    ClipStart = table.Column<decimal>(type: "decimal(20,10)", precision: 20, scale: 10, nullable: false),
                    ClipDuration = table.Column<decimal>(type: "decimal(20,10)", precision: 20, scale: 10, nullable: false),
                    TopicId = table.Column<int>(type: "int", nullable: false),
                    VideoId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoKeyword", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoKeyword_VideoTopic_TopicId",
                        column: x => x.TopicId,
                        principalTable: "VideoTopic",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VideoKeyword_Video_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Video",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Video_Name",
                table: "Video",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VideoKeyword_Name_TopicId_VideoId_ClipStart",
                table: "VideoKeyword",
                columns: new[] { "Name", "TopicId", "VideoId", "ClipStart" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VideoTopic_Number_VideoId",
                table: "VideoTopic",
                columns: new[] { "Number", "VideoId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoKeyword");

            migrationBuilder.DropTable(
                name: "VideoTopic");

            migrationBuilder.DropTable(
                name: "Video");
        }
    }
}
