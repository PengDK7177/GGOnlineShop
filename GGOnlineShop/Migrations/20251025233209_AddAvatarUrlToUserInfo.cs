using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGOnlineShop.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarUrlToUserInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "UserInfos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateTime",
                value: new DateTime(2025, 10, 26, 7, 32, 8, 884, DateTimeKind.Local).AddTicks(2204));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateTime",
                value: new DateTime(2025, 10, 26, 7, 32, 8, 884, DateTimeKind.Local).AddTicks(2247));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreateTime",
                value: new DateTime(2025, 10, 26, 7, 32, 8, 884, DateTimeKind.Local).AddTicks(2249));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreateTime",
                value: new DateTime(2025, 10, 26, 7, 32, 8, 884, DateTimeKind.Local).AddTicks(2267));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreateTime",
                value: new DateTime(2025, 10, 26, 7, 32, 8, 884, DateTimeKind.Local).AddTicks(2268));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreateTime",
                value: new DateTime(2025, 10, 26, 7, 32, 8, 884, DateTimeKind.Local).AddTicks(2269));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreateTime",
                value: new DateTime(2025, 10, 26, 7, 32, 8, 884, DateTimeKind.Local).AddTicks(2291));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreateTime",
                value: new DateTime(2025, 10, 26, 7, 32, 8, 884, DateTimeKind.Local).AddTicks(2292));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreateTime",
                value: new DateTime(2025, 10, 26, 7, 32, 8, 884, DateTimeKind.Local).AddTicks(2293));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "UserInfos");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateTime",
                value: new DateTime(2025, 10, 26, 0, 51, 30, 668, DateTimeKind.Local).AddTicks(2511));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateTime",
                value: new DateTime(2025, 10, 26, 0, 51, 30, 668, DateTimeKind.Local).AddTicks(2586));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreateTime",
                value: new DateTime(2025, 10, 26, 0, 51, 30, 668, DateTimeKind.Local).AddTicks(2589));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreateTime",
                value: new DateTime(2025, 10, 26, 0, 51, 30, 668, DateTimeKind.Local).AddTicks(2605));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreateTime",
                value: new DateTime(2025, 10, 26, 0, 51, 30, 668, DateTimeKind.Local).AddTicks(2607));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreateTime",
                value: new DateTime(2025, 10, 26, 0, 51, 30, 668, DateTimeKind.Local).AddTicks(2609));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreateTime",
                value: new DateTime(2025, 10, 26, 0, 51, 30, 668, DateTimeKind.Local).AddTicks(2626));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreateTime",
                value: new DateTime(2025, 10, 26, 0, 51, 30, 668, DateTimeKind.Local).AddTicks(2628));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreateTime",
                value: new DateTime(2025, 10, 26, 0, 51, 30, 668, DateTimeKind.Local).AddTicks(2630));
        }
    }
}
