using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace N10.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddChronicles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chronicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Day = table.Column<byte>(type: "tinyint", nullable: true),
                    Month = table.Column<byte>(type: "tinyint", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    EnhancedContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chronicles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chronicles_Date",
                table: "Chronicles",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Chronicles_Month_Day",
                table: "Chronicles",
                columns: new[] { "Month", "Day" });

            migrationBuilder.CreateIndex(
                name: "IX_Chronicles_Year_Month_Day",
                table: "Chronicles",
                columns: new[] { "Year", "Month", "Day" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chronicles");
        }
    }
}
