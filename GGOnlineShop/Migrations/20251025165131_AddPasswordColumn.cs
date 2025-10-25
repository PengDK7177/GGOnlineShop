using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGOnlineShop.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Users_UserId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCards_Users_UserId",
                table: "UserCards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "UserInfos");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Username",
                table: "UserInfos",
                newName: "IX_UserInfos_Username");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "UserInfos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserInfos",
                table: "UserInfos",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_UserInfos_UserId",
                table: "CartItems",
                column: "UserId",
                principalTable: "UserInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCards_UserInfos_UserId",
                table: "UserCards",
                column: "UserId",
                principalTable: "UserInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_UserInfos_UserId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCards_UserInfos_UserId",
                table: "UserCards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserInfos",
                table: "UserInfos");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "UserInfos");

            migrationBuilder.RenameTable(
                name: "UserInfos",
                newName: "Users");

            migrationBuilder.RenameIndex(
                name: "IX_UserInfos_Username",
                table: "Users",
                newName: "IX_Users_Username");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateTime",
                value: new DateTime(2025, 10, 25, 4, 59, 15, 600, DateTimeKind.Local).AddTicks(7257));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateTime",
                value: new DateTime(2025, 10, 25, 4, 59, 15, 600, DateTimeKind.Local).AddTicks(7303));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreateTime",
                value: new DateTime(2025, 10, 25, 4, 59, 15, 600, DateTimeKind.Local).AddTicks(7305));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreateTime",
                value: new DateTime(2025, 10, 25, 4, 59, 15, 600, DateTimeKind.Local).AddTicks(7327));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreateTime",
                value: new DateTime(2025, 10, 25, 4, 59, 15, 600, DateTimeKind.Local).AddTicks(7329));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreateTime",
                value: new DateTime(2025, 10, 25, 4, 59, 15, 600, DateTimeKind.Local).AddTicks(7331));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreateTime",
                value: new DateTime(2025, 10, 25, 4, 59, 15, 600, DateTimeKind.Local).AddTicks(7354));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreateTime",
                value: new DateTime(2025, 10, 25, 4, 59, 15, 600, DateTimeKind.Local).AddTicks(7356));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreateTime",
                value: new DateTime(2025, 10, 25, 4, 59, 15, 600, DateTimeKind.Local).AddTicks(7358));

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Users_UserId",
                table: "CartItems",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCards_Users_UserId",
                table: "UserCards",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
