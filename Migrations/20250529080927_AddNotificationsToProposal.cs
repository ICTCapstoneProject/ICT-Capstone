using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagerMvc.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationsToProposal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "role",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "notification_type",
                table: "notifications",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "proposal_id",
                table: "notifications",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_notifications_proposal_id",
                table: "notifications",
                column: "proposal_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_user_id",
                table: "notifications",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_proposals_proposal_id",
                table: "notifications",
                column: "proposal_id",
                principalTable: "proposals",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_users_user_id",
                table: "notifications",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notifications_proposals_proposal_id",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_notifications_users_user_id",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_notifications_proposal_id",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_notifications_user_id",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "method_image",
                table: "proposals");

            migrationBuilder.DropColumn(
                name: "notification_type",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "proposal_id",
                table: "notifications");

            migrationBuilder.AddColumn<string>(
                name: "role",
                table: "users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
