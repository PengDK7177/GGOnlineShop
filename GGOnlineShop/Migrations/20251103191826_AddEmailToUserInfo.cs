using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGOnlineShop.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailToUserInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateTime",
                value: new DateTime(2025, 11, 4, 3, 18, 26, 441, DateTimeKind.Local).AddTicks(2660));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateTime",
                value: new DateTime(2025, 11, 4, 3, 18, 26, 441, DateTimeKind.Local).AddTicks(2698));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreateTime",
                value: new DateTime(2025, 11, 4, 3, 18, 26, 441, DateTimeKind.Local).AddTicks(2700));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreateTime",
                value: new DateTime(2025, 11, 4, 3, 18, 26, 441, DateTimeKind.Local).AddTicks(2719));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreateTime",
                value: new DateTime(2025, 11, 4, 3, 18, 26, 441, DateTimeKind.Local).AddTicks(2720));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreateTime",
                value: new DateTime(2025, 11, 4, 3, 18, 26, 441, DateTimeKind.Local).AddTicks(2721));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreateTime",
                value: new DateTime(2025, 11, 4, 3, 18, 26, 441, DateTimeKind.Local).AddTicks(2796));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreateTime",
                value: new DateTime(2025, 11, 4, 3, 18, 26, 441, DateTimeKind.Local).AddTicks(2797));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreateTime",
                value: new DateTime(2025, 11, 4, 3, 18, 26, 441, DateTimeKind.Local).AddTicks(2799));

            // 1) Normalize empty / NULL emails to unique placeholders to avoid duplicates
            migrationBuilder.Sql(@"
-- Make empty/null emails unique placeholders using Username+Id
UPDATE UserInfos
SET Email = Username + '_user' + CAST(Id AS nvarchar(20)) + '@local.test'
WHERE Email IS NULL OR Email = '';

-- For exact duplicate emails keep the first and change others by appending the Id
;WITH Dups AS (
    SELECT Id, Email,
           ROW_NUMBER() OVER (PARTITION BY Email ORDER BY Id) rn
    FROM UserInfos
    WHERE Email IS NOT NULL AND Email <> ''
)
UPDATE u
SET Email = u.Email + '_dup' + CAST(u.Id AS nvarchar(20)) + '@local.test'
FROM UserInfos u
INNER JOIN Dups d ON u.Id = d.Id
WHERE d.rn > 1;
");

            // 2) Create a filtered unique index that ignores NULL/empty values
            migrationBuilder.CreateIndex(
                name: "IX_UserInfos_Email",
                table: "UserInfos",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL AND Email <> ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DropIndex(
                name: "IX_UserInfos_Email",
                table: "UserInfos");
        }
    }
}
