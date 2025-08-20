using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Forename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DateOfBirth", "Email", "Forename", "IsActive", "Surname" },
                values: new object[,]
                {
                    { 1L, new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ploew@example.com", "Peter", true, "Loew" },
                    { 2L, new DateTime(1980, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "bfgates@example.com", "Benjamin Franklin", true, "Gates" },
                    { 3L, new DateTime(1986, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "ctroy@example.com", "Castor", false, "Troy" },
                    { 4L, new DateTime(1984, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "mraines@example.com", "Memphis", true, "Raines" },
                    { 5L, new DateTime(1989, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "sgodspeed@example.com", "Stanley", true, "Goodspeed" },
                    { 6L, new DateTime(1982, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "himcdunnough@example.com", "H.I.", true, "McDunnough" },
                    { 7L, new DateTime(1981, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "cpoe@example.com", "Cameron", false, "Poe" },
                    { 8L, null, "emalus@example.com", "Edward", false, "Malus" },
                    { 9L, null, "dmacready@example.com", "Damon", false, "Macready" },
                    { 10L, null, "jblaze@example.com", "Johnny", true, "Blaze" },
                    { 11L, null, "rfeld@example.com", "Robin", true, "Feld" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
