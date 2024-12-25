using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZeynepBeautySaloon.Migrations
{
    /// <inheritdoc />
    public partial class TablolarGuncel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Uyeler_UyeId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Islemler_Personeller_PersonelId",
                table: "Islemler");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Uyeler",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "Durum",
                table: "Personeller",
                newName: "MusaitlikDurumu");

            migrationBuilder.AlterColumn<int>(
                name: "PersonelId",
                table: "Islemler",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "Ucret",
                table: "Appointments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Uyeler_UyeId",
                table: "Appointments",
                column: "UyeId",
                principalTable: "Uyeler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Islemler_Personeller_PersonelId",
                table: "Islemler",
                column: "PersonelId",
                principalTable: "Personeller",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Uyeler_UyeId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Islemler_Personeller_PersonelId",
                table: "Islemler");

            migrationBuilder.DropColumn(
                name: "Ucret",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Uyeler",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "MusaitlikDurumu",
                table: "Personeller",
                newName: "Durum");

            migrationBuilder.AlterColumn<int>(
                name: "PersonelId",
                table: "Islemler",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Uyeler_UyeId",
                table: "Appointments",
                column: "UyeId",
                principalTable: "Uyeler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Islemler_Personeller_PersonelId",
                table: "Islemler",
                column: "PersonelId",
                principalTable: "Personeller",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
