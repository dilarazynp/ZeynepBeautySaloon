using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZeynepBeautySaloon.Migrations
{
    /// <inheritdoc />
    public partial class GuncelTablolar1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OnayDurumu",
                table: "Randevular",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnayDurumu",
                table: "Randevular");
        }
    }
}
