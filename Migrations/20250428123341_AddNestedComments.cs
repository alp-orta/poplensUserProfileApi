using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace poplensUserProfileApi.Migrations
{
    /// <inheritdoc />
    public partial class AddNestedComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentCommentId",
                schema: "public",
                table: "Comments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentCommentId",
                schema: "public",
                table: "Comments",
                column: "ParentCommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                schema: "public",
                table: "Comments",
                column: "ParentCommentId",
                principalSchema: "public",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                schema: "public",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ParentCommentId",
                schema: "public",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ParentCommentId",
                schema: "public",
                table: "Comments");
        }
    }
}
