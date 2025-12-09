using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SecureTaskHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    OwnerUserId = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "07e5fcac-8993-438e-873f-8acfbce532dd", "2f4e9d57-8d6b-5c4f-0e3g-7b9d5c8f6e4b", "Admin", "ADMIN" },
                    { "20fd6e8a-2b25-4efa-a109-a67f339d5402", "1e3f8d46-7c5a-4b3e-9d2f-6a8c4b7e5d3a", "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "21809ee3-a838-42e1-99b5-ad07b8deb31b", 0, "83ef9ad3-c995-479a-ac87-f41ef09796b6", "bob@demo.local", true, false, null, "BOB@DEMO.LOCAL", "BOB@DEMO.LOCAL", "AQAAAAIAAYagAAAAEM3YmPjAE7F+MGthkwPbzGpY7c6Ucwv/qxsLCR5UYZUgHZ9LyNsh5Fx8ebTvopXCWA==", null, false, "4318e961-2a50-43fd-8f29-298a814a90fc", false, "bob@demo.local" },
                    { "495d56e4-43b1-4e54-9cef-10e917d28f63", 0, "054ec42a-c1df-4b3f-ad55-f300ec6a853a", "alice@demo.local", true, false, null, "ALICE@DEMO.LOCAL", "ALICE@DEMO.LOCAL", "AQAAAAIAAYagAAAAECYgX4PAQWRU8bzBR+6KquSbnLMIMERJ9qqNn/byfYokNqlGXORsBzlz5bT7Cz6qVA==", null, false, "1a00fd73-7f7a-4719-9fd9-0e0d6a148efd", false, "alice@demo.local" },
                    { "d633d3fa-5890-413d-a0b7-242601a49a75", 0, "47287b02-ff20-4dc8-bf50-69da9d1a656a", "admin@demo.local", true, false, null, "ADMIN@DEMO.LOCAL", "ADMIN@DEMO.LOCAL", "AQAAAAIAAYagAAAAEMP818yxP3ieYL8yLcqz610LhpuIq2as0NSTCr7SD9/dwM3Zq/MehPV16S/MG9bshA==", null, false, "38775744-6fe3-4f87-b8c8-ea3fcd7eb089", false, "admin@demo.local" }
                });

            migrationBuilder.InsertData(
                table: "TaskItems",
                columns: new[] { "Id", "CreatedAt", "Description", "OwnerUserId", "Status", "Title" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 11, 24, 0, 0, 0, 0, DateTimeKind.Utc), "Complete quarterly security audit review", "d633d3fa-5890-413d-a0b7-242601a49a75", "InProgress", "Review system security audit" },
                    { 2, new DateTime(2025, 11, 26, 0, 0, 0, 0, DateTimeKind.Utc), "Add new reporting features to admin dashboard", "d633d3fa-5890-413d-a0b7-242601a49a75", "New", "Update admin dashboard" },
                    { 3, new DateTime(2025, 11, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Write and submit Q1 project proposal", "495d56e4-43b1-4e54-9cef-10e917d28f63", "InProgress", "Complete project proposal" },
                    { 4, new DateTime(2025, 11, 27, 0, 0, 0, 0, DateTimeKind.Utc), "Review pull requests from team members", "495d56e4-43b1-4e54-9cef-10e917d28f63", "New", "Review code changes" },
                    { 5, new DateTime(2025, 11, 19, 0, 0, 0, 0, DateTimeKind.Utc), "Organize monthly team sync", "495d56e4-43b1-4e54-9cef-10e917d28f63", "Done", "Schedule team meeting" },
                    { 6, new DateTime(2025, 11, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Debug and fix authentication issue reported by users", "21809ee3-a838-42e1-99b5-ad07b8deb31b", "InProgress", "Fix login bug" },
                    { 7, new DateTime(2025, 11, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Document new API endpoints for external developers", "21809ee3-a838-42e1-99b5-ad07b8deb31b", "New", "Write API documentation" },
                    { 8, new DateTime(2025, 11, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Update NuGet packages to latest stable versions", "21809ee3-a838-42e1-99b5-ad07b8deb31b", "Done", "Update dependencies" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "20fd6e8a-2b25-4efa-a109-a67f339d5402", "21809ee3-a838-42e1-99b5-ad07b8deb31b" },
                    { "20fd6e8a-2b25-4efa-a109-a67f339d5402", "495d56e4-43b1-4e54-9cef-10e917d28f63" },
                    { "07e5fcac-8993-438e-873f-8acfbce532dd", "d633d3fa-5890-413d-a0b7-242601a49a75" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_OwnerUserId",
                table: "TaskItems",
                column: "OwnerUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "TaskItems");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
