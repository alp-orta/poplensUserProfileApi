using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace poplensUserProfileApi.Migrations
{
    /// <inheritdoc />
    public partial class makeembeddingnullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                schema: "public",
                table: "Reviews",
                type: "vector(1536)",
                nullable: true,
                oldClrType: typeof(Vector),
                oldType: "vector(1536)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                schema: "public",
                table: "Reviews",
                type: "vector(1536)",
                nullable: false,
                oldClrType: typeof(Vector),
                oldType: "vector(1536)",
                oldNullable: true);
        }
    }
}
