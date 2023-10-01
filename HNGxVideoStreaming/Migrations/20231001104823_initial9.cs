using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HNGxVideoStreaming.Migrations
{
    /// <inheritdoc />
    public partial class initial9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadKey",
                table: "TranscribeDatas");

            migrationBuilder.AddColumn<int>(
                name: "UploadKeyId",
                table: "TranscribeDatas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TranscribeDatas_UploadKeyId",
                table: "TranscribeDatas",
                column: "UploadKeyId");

            migrationBuilder.AddForeignKey(
                name: "FK_TranscribeDatas_UploadContexts_UploadKeyId",
                table: "TranscribeDatas",
                column: "UploadKeyId",
                principalTable: "UploadContexts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TranscribeDatas_UploadContexts_UploadKeyId",
                table: "TranscribeDatas");

            migrationBuilder.DropIndex(
                name: "IX_TranscribeDatas_UploadKeyId",
                table: "TranscribeDatas");

            migrationBuilder.DropColumn(
                name: "UploadKeyId",
                table: "TranscribeDatas");

            migrationBuilder.AddColumn<string>(
                name: "UploadKey",
                table: "TranscribeDatas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
