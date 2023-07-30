using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurSunday.Migrations
{
    /// <inheritdoc />
    public partial class thumbnailAddedinpage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "Pages",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "Pages");
        }
    }
}
