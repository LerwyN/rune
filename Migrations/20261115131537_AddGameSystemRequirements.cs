using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RUNE.Migrations
{
    /// <inheritdoc />
    public partial class AddGameSystemRequirements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameSystemRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameId = table.Column<int>(type: "integer", nullable: false),
                    MinimumOS = table.Column<string>(type: "text", nullable: true),
                    MinimumProcessor = table.Column<string>(type: "text", nullable: true),
                    MinimumMemory = table.Column<string>(type: "text", nullable: true),
                    MinimumGraphics = table.Column<string>(type: "text", nullable: true),
                    MinimumStorage = table.Column<string>(type: "text", nullable: true),
                    RecommendedOS = table.Column<string>(type: "text", nullable: true),
                    RecommendedProcessor = table.Column<string>(type: "text", nullable: true),
                    RecommendedMemory = table.Column<string>(type: "text", nullable: true),
                    RecommendedGraphics = table.Column<string>(type: "text", nullable: true),
                    RecommendedStorage = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSystemRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameSystemRequirements_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameSystemRequirements_GameId",
                table: "GameSystemRequirements",
                column: "GameId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameSystemRequirements");
        }
    }
}
