using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagerMvc.Migrations
{
    /// <inheritdoc />
    public partial class AddProposalIdToComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "role",
                table: "users");

            migrationBuilder.DropColumn(
                name: "resources",
                table: "proposals");

            migrationBuilder.DropColumn(
                name: "uploaded_at",
                table: "attachments");

            // migrationBuilder.RenameColumn(
            //     name: "file_url",
            //     table: "attachments",
            //     newName: "file_path");

            // migrationBuilder.RenameColumn(
            //     name: "uploaded_by",
            //     table: "attachments",
            //     newName: "type_id");

            // migrationBuilder.AddColumn<int>(
            //     name: "lead_researcher_id",
            //     table: "proposals",
            //     type: "INTEGER",
            //     nullable: false,
            //     defaultValue: 0);

            // migrationBuilder.AddColumn<string>(
            //     name: "physical_resources",
            //     table: "proposals",
            //     type: "TEXT",
            //     maxLength: 250,
            //     nullable: false,
            //     defaultValue: "");

            // migrationBuilder.AddColumn<string>(
            //     name: "notification_type",
            //     table: "notifications",
            //     type: "TEXT",
            //     nullable: false,
            //     defaultValue: "");

            // migrationBuilder.AddColumn<int>(
            //     name: "proposal_id",
            //     table: "notifications",
            //     type: "INTEGER",
            //     nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "proposal_id",
                table: "comments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            // migrationBuilder.CreateTable(
            //     name: "attachment_types",
            //     columns: table => new
            //     {
            //         type_id = table.Column<int>(type: "INTEGER", nullable: false)
            //             .Annotation("Sqlite:Autoincrement", true),
            //         type_name = table.Column<string>(type: "TEXT", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_attachment_types", x => x.type_id);
            //     });

            // migrationBuilder.CreateTable(
            //     name: "financial_resources",
            //     columns: table => new
            //     {
            //         id = table.Column<int>(type: "INTEGER", nullable: false)
            //             .Annotation("Sqlite:Autoincrement", true),
            //         proposal_id = table.Column<int>(type: "INTEGER", nullable: false),
            //         title = table.Column<string>(type: "TEXT", nullable: false),
            //         cost = table.Column<decimal>(type: "TEXT", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_financial_resources", x => x.id);
            //         table.ForeignKey(
            //             name: "FK_financial_resources_proposals_proposal_id",
            //             column: x => x.proposal_id,
            //             principalTable: "proposals",
            //             principalColumn: "id",
            //             onDelete: ReferentialAction.Cascade);
            //     });

            // migrationBuilder.CreateTable(
            //     name: "proposal_researchers",
            //     columns: table => new
            //     {
            //         proposal_id = table.Column<int>(type: "INTEGER", nullable: false),
            //         user_id = table.Column<int>(type: "INTEGER", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_proposal_researchers", x => new { x.proposal_id, x.user_id });
            //         table.ForeignKey(
            //             name: "FK_proposal_researchers_proposals_proposal_id",
            //             column: x => x.proposal_id,
            //             principalTable: "proposals",
            //             principalColumn: "id",
            //             onDelete: ReferentialAction.Cascade);
            //         table.ForeignKey(
            //             name: "FK_proposal_researchers_users_user_id",
            //             column: x => x.user_id,
            //             principalTable: "users",
            //             principalColumn: "user_id",
            //             onDelete: ReferentialAction.Cascade);
            //     });

            // migrationBuilder.CreateTable(
            //     name: "ProposalHistories",
            //     columns: table => new
            //     {
            //         HistoryId = table.Column<int>(type: "INTEGER", nullable: false)
            //             .Annotation("Sqlite:Autoincrement", true),
            //         ProposalId = table.Column<int>(type: "INTEGER", nullable: false),
            //         Title = table.Column<string>(type: "TEXT", nullable: false),
            //         Synopsis = table.Column<string>(type: "TEXT", nullable: false),
            //         Method = table.Column<string>(type: "TEXT", nullable: false),
            //         ProjectLevelId = table.Column<int>(type: "INTEGER", nullable: false),
            //         PhysicalResources = table.Column<string>(type: "TEXT", nullable: false),
            //         EthicalConsiderations = table.Column<string>(type: "TEXT", nullable: false),
            //         Outcomes = table.Column<string>(type: "TEXT", nullable: false),
            //         Milestones = table.Column<string>(type: "TEXT", nullable: false),
            //         EstimatedCompletionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
            //         LeadResearcherId = table.Column<int>(type: "INTEGER", nullable: false),
            //         StatusId = table.Column<int>(type: "INTEGER", nullable: false),
            //         Action = table.Column<string>(type: "TEXT", nullable: false),
            //         Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
            //         ChangedBy = table.Column<int>(type: "INTEGER", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_ProposalHistories", x => x.HistoryId);
            //     });

            // migrationBuilder.CreateTable(
            //     name: "user_roles",
            //     columns: table => new
            //     {
            //         user_id = table.Column<int>(type: "INTEGER", nullable: false),
            //         role_id = table.Column<int>(type: "INTEGER", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_user_roles", x => new { x.user_id, x.role_id });
            //         table.ForeignKey(
            //             name: "FK_user_roles_roles_role_id",
            //             column: x => x.role_id,
            //             principalTable: "roles",
            //             principalColumn: "role_id",
            //             onDelete: ReferentialAction.Cascade);
            //         table.ForeignKey(
            //             name: "FK_user_roles_users_user_id",
            //             column: x => x.user_id,
            //             principalTable: "users",
            //             principalColumn: "user_id",
            //             onDelete: ReferentialAction.Cascade);
            //     });

            migrationBuilder.CreateIndex(
                name: "IX_notifications_proposal_id",
                table: "notifications",
                column: "proposal_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_user_id",
                table: "notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_comments_proposal_id",
                table: "comments",
                column: "proposal_id");

            migrationBuilder.CreateIndex(
                name: "IX_attachments_proposal_id",
                table: "attachments",
                column: "proposal_id");

            migrationBuilder.CreateIndex(
                name: "IX_attachments_type_id",
                table: "attachments",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "IX_financial_resources_proposal_id",
                table: "financial_resources",
                column: "proposal_id");

            migrationBuilder.CreateIndex(
                name: "IX_proposal_researchers_user_id",
                table: "proposal_researchers",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.AddForeignKey(
                name: "FK_attachments_attachment_types_type_id",
                table: "attachments",
                column: "type_id",
                principalTable: "attachment_types",
                principalColumn: "type_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_attachments_proposals_proposal_id",
                table: "attachments",
                column: "proposal_id",
                principalTable: "proposals",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_proposals_proposal_id",
                table: "comments",
                column: "proposal_id",
                principalTable: "proposals",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_attachments_attachment_types_type_id",
                table: "attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_attachments_proposals_proposal_id",
                table: "attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_proposals_proposal_id",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_notifications_proposals_proposal_id",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_notifications_users_user_id",
                table: "notifications");

            migrationBuilder.DropTable(
                name: "attachment_types");

            migrationBuilder.DropTable(
                name: "financial_resources");

            migrationBuilder.DropTable(
                name: "proposal_researchers");

            migrationBuilder.DropTable(
                name: "ProposalHistories");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropIndex(
                name: "IX_notifications_proposal_id",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_notifications_user_id",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_comments_proposal_id",
                table: "comments");

            migrationBuilder.DropIndex(
                name: "IX_attachments_proposal_id",
                table: "attachments");

            migrationBuilder.DropIndex(
                name: "IX_attachments_type_id",
                table: "attachments");

            migrationBuilder.DropColumn(
                name: "lead_researcher_id",
                table: "proposals");

            migrationBuilder.DropColumn(
                name: "physical_resources",
                table: "proposals");

            migrationBuilder.DropColumn(
                name: "notification_type",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "proposal_id",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "proposal_id",
                table: "comments");

            migrationBuilder.RenameColumn(
                name: "file_path",
                table: "attachments",
                newName: "file_url");

            migrationBuilder.RenameColumn(
                name: "type_id",
                table: "attachments",
                newName: "uploaded_by");

            migrationBuilder.AddColumn<string>(
                name: "role",
                table: "users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "resources",
                table: "proposals",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "uploaded_at",
                table: "attachments",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
