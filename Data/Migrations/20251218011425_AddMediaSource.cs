using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace N10.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MediaSourceId",
                table: "Movies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MediaSources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BasePath = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaSources", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movies_FileName",
                table: "Movies",
                column: "FileName");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_MediaSourceId",
                table: "Movies",
                column: "MediaSourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_MediaSources_MediaSourceId",
                table: "Movies",
                column: "MediaSourceId",
                principalTable: "MediaSources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_MediaSources_MediaSourceId",
                table: "Movies");

            migrationBuilder.DropTable(
                name: "MediaSources");

            migrationBuilder.DropIndex(
                name: "IX_Movies_FileName",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_MediaSourceId",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "MediaSourceId",
                table: "Movies");
        }
    }
}
