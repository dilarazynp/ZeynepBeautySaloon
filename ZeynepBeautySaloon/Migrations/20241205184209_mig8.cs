using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZeynepBeautySaloon.Migrations
{
    /// <inheritdoc />
    public partial class mig8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Islemler_PersonelId",
                table: "Islemler");

            migrationBuilder.DropColumn(
                name: "Islemler",
                table: "Personeller");

            migrationBuilder.CreateIndex(
                name: "IX_Islemler_PersonelId",
                table: "Islemler",
                column: "PersonelId",
                unique: true,
                filter: "[PersonelId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Islemler_PersonelId",
                table: "Islemler");

            migrationBuilder.AddColumn<string>(
                name: "Islemler",
                table: "Personeller",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Islemler_PersonelId",
                table: "Islemler",
                column: "PersonelId");
        }
    }
}
