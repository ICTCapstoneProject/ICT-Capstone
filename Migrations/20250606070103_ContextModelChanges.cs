using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagerMvc.Migrations
{
    /// <inheritdoc />
    public partial class ContextModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.AddColumn<int>(
            //     name: "UserId",
            //     table: "comments",
            //     type: "INTEGER",
            //     nullable: false,
            //     defaultValue: 0);

            // migrationBuilder.CreateIndex(
            //     name: "IX_comments_UserId",
            //     table: "comments",
            //     column: "UserId");

            // migrationBuilder.AddForeignKey(
            //     name: "FK_comments_users_UserId",
            //     table: "comments",
            //     column: "UserId",
            //     principalTable: "users",
            //     principalColumn: "user_id",
            //     onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comments_users_UserId",
                table: "comments");

            migrationBuilder.DropIndex(
                name: "IX_comments_UserId",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "comments");
        }
    }
}
