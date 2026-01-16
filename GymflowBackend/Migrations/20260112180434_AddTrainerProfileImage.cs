using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymFlowBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainerProfileImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "Trainers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "Trainers");
        }
    }
}
