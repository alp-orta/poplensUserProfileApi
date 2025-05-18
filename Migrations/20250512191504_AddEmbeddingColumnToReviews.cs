using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace poplensUserProfileApi.Migrations
{
    /// <inheritdoc />
    public partial class AddEmbeddingColumnToReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.AddColumn<Vector>(
                name: "Embedding",
                schema: "public",
                table: "Reviews",
                type: "vector(1536)",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                schema: "public",
                table: "ReviewDetail",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Embedding",
                schema: "public",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Username",
                schema: "public",
                table: "ReviewDetail");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:vector", ",,");
        }
    }
}
