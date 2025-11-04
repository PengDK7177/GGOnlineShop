using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGOnlineShop.Migrations
{
    public partial class AddIsAdminToUserInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // add Email as nullable (do NOT set default empty string)
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "UserInfos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            // add IsAdmin with default false
            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "UserInfos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "SubCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategories", x => x.Id);
                    migrationBuilder.CreateIndex(
                        name: "IX_SubCategories_CategoryId",
                        table: "SubCategories",
                        column: "CategoryId");
                    table.ForeignKey(
                        name: "FK_SubCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // update Category timestamps (kept from original migration)
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateTime",
                value: new DateTime(2025, 11, 4, 3, 1, 31, 991, DateTimeKind.Local).AddTicks(5776));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateTime",
                value: new DateTime(2025, 11, 4, 3, 1, 31, 991, DateTimeKind.Local).AddTicks(5812));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreateTime",
                value: new DateTime(2025, 11, 4, 3, 1, 31, 991, DateTimeKind.Local).AddTicks(5814));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreateTime",
                value: new DateTime(2025, 11, 4, 3, 1, 31, 991, DateTimeKind.Local).AddTicks(5832));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreateTime",
                value: new DateTime(2025, 11, 4, 3, 1, 31, 991, DateTimeKind.Local).AddTicks(5833));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreateTime",
                value: new DateTime(2025, 11, 4, 3, 1, 31, 991, DateTimeKind.Local).AddTicks(5835));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreateTime",
                value: new DateTime(2025, 11, 4, 3, 1, 31, 991, DateTimeKind.Local).AddTicks(5853));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreateTime",
                value: new DateTime(2025, 11, 4, 3, 1, 31, 991, DateTimeKind.Local).AddTicks(5854));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreateTime",
                value: new DateTime(2025, 11, 4, 3, 1, 31, 991, DateTimeKind.Local).AddTicks(5855));

            // Safely drop existing index if present, then create filtered unique index
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1 FROM sys.indexes i
    JOIN sys.objects o ON i.object_id = o.object_id
    WHERE i.name = N'IX_UserInfos_Email' AND o.name = N'UserInfos'
)
BEGIN
    DROP INDEX IX_UserInfos_Email ON dbo.UserInfos;
END
");

            migrationBuilder.CreateIndex(
                name: "IX_UserInfos_Email",
                table: "UserInfos",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL AND Email <> ''");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubCategories");

            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1 FROM sys.indexes i
    JOIN sys.objects o ON i.object_id = o.object_id
    WHERE i.name = N'IX_UserInfos_Email' AND o.name = N'UserInfos'
)
BEGIN
    DROP INDEX IX_UserInfos_Email ON dbo.UserInfos;
END
");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "UserInfos");

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "UserInfos");

            // restore original Category timestamps if needed...
        }
    }
}
