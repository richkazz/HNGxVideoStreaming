using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HNGxVideoStreaming.Migrations
{
    /// <inheritdoc />
    public partial class initial8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioUrl",
                table: "UploadContexts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AudioUrl",
                table: "UploadContexts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
