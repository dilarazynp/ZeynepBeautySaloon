using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZeynepBeautySaloon.Migrations
{
    /// <inheritdoc />
    public partial class AddRolToUye : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Islemler_Personeller_PersonelId",
                table: "Islemler");

            migrationBuilder.DropIndex(
                name: "IX_Islemler_PersonelId",
                table: "Islemler");

            migrationBuilder.AddColumn<string>(
                name: "Rol",
                table: "Uyeler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "PersonelId",
                table: "Islemler",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Islemler_PersonelId",
                table: "Islemler",
                column: "PersonelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Islemler_Personeller_PersonelId",
                table: "Islemler",
                column: "PersonelId",
                principalTable: "Personeller",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Islemler_Personeller_PersonelId",
                table: "Islemler");

            migrationBuilder.DropIndex(
                name: "IX_Islemler_PersonelId",
                table: "Islemler");

            migrationBuilder.DropColumn(
                name: "Rol",
                table: "Uyeler");

            migrationBuilder.AlterColumn<int>(
                name: "PersonelId",
                table: "Islemler",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Islemler_PersonelId",
                table: "Islemler",
                column: "PersonelId",
                unique: true,
                filter: "[PersonelId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Islemler_Personeller_PersonelId",
                table: "Islemler",
                column: "PersonelId",
                principalTable: "Personeller",
                principalColumn: "Id");
        }
    }
}
