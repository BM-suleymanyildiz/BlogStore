using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogStore.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddToxicityScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ToxicityScore",
                table: "Comments",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ToxicityScore",
                table: "Comments");
        }
    }
} 