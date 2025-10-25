using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GGOnlineShop.Migrations
{
    /// <inheritdoc />
    public partial class SeedCategoriesTB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Age = table.Column<int>(type: "int", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FavoriteTeam = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    PlayerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Team = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardYear = table.Column<int>(type: "int", nullable: false),
                    CardGrade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CardStock = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CardDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardImg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cards_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    CardId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CardId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => new { x.CardId, x.UserId });
                    table.ForeignKey(
                        name: "FK_CartItems_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_Cards_CardId1",
                        column: x => x.CardId1,
                        principalTable: "Cards",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CartItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CardId = table.Column<int>(type: "int", nullable: false),
                    HoldNum = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    IsSell = table.Column<bool>(type: "bit", nullable: false),
                    SellPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCards_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCards_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreateTime", "Name", "ParentId", "SortOrder" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 25, 4, 59, 15, 600, DateTimeKind.Local).AddTicks(7257), "All", null, 1 },
                    { 2, new DateTime(2025, 10, 25, 4, 59, 15, 600, DateTimeKind.Local).AddTicks(7303), "Basketball", 1, 2 },
                    { 3, new DateTime(2025, 10, 25, 4, 59, 15, 600, DateTimeKind.Local).AddTicks(7305), "Football", 1, 3 },
                    { 4, new DateTime(2025, 10, 25, 4, 59, 15, 600, DateTimeKind.Local).AddTicks(7327), "NBA", 2, 4 },
                    { 5, new DateTime(2025, 10, 25, 4, 59, 15, 600, DateTimeKind.Local).AddTicks(7329), "FIBA", 2, 5 },
                    { 6, new DateTime(2025, 10, 25, 4, 59, 15, 600, DateTimeKind.Local).AddTicks(7331), "CBA", 2, 6 },
                    { 7, new DateTime(2025, 10, 25, 4, 59, 15, 600, DateTimeKind.Local).AddTicks(7354), "Serie A", 3, 7 },
                    { 8, new DateTime(2025, 10, 25, 4, 59, 15, 600, DateTimeKind.Local).AddTicks(7356), "La Liga", 3, 8 },
                    { 9, new DateTime(2025, 10, 25, 4, 59, 15, 600, DateTimeKind.Local).AddTicks(7358), "Premier League", 3, 9 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CategoryId",
                table: "Cards",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_PlayerName",
                table: "Cards",
                column: "PlayerName");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CardId1",
                table: "CartItems",
                column: "CardId1");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_UserId",
                table: "CartItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentId",
                table: "Categories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCards_CardId",
                table: "UserCards",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCards_IsSell",
                table: "UserCards",
                column: "IsSell");

            migrationBuilder.CreateIndex(
                name: "IX_UserCards_UserId_CardId",
                table: "UserCards",
                columns: new[] { "UserId", "CardId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "UserCards");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
