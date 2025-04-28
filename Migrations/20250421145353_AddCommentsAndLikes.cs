using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace poplensUserProfileApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentsAndLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.RenameTable(
                name: "Reviews",
                newName: "Reviews",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Profiles",
                newName: "Profiles",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Follows",
                newName: "Follows",
                newSchema: "public");

            migrationBuilder.CreateTable(
                name: "Comments",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReviewId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalSchema: "public",
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReviewId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Likes_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalSchema: "public",
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewDetail",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Rating = table.Column<float>(type: "real", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    MediaId = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MediaTitle = table.Column<string>(type: "text", nullable: false),
                    MediaType = table.Column<string>(type: "text", nullable: false),
                    MediaCachedImagePath = table.Column<string>(type: "text", nullable: false),
                    MediaCreator = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewDetail_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "public",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ReviewId",
                schema: "public",
                table: "Comments",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_ReviewId_ProfileId",
                schema: "public",
                table: "Likes",
                columns: new[] { "ReviewId", "ProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewDetail_ProfileId",
                schema: "public",
                table: "ReviewDetail",
                column: "ProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Likes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ReviewDetail",
                schema: "public");

            migrationBuilder.RenameTable(
                name: "Reviews",
                schema: "public",
                newName: "Reviews");

            migrationBuilder.RenameTable(
                name: "Profiles",
                schema: "public",
                newName: "Profiles");

            migrationBuilder.RenameTable(
                name: "Follows",
                schema: "public",
                newName: "Follows");
        }
    }
}
