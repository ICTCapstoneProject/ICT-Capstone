﻿// <auto-generated />
using System;
using FSSA.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ProjectManagerMvc.Migrations
{
    [DbContext(typeof(ProjectManagerContext))]
    partial class ProjectManagerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.5");

            modelBuilder.Entity("FSSA.Models.Attachment", b =>
                {
                    b.Property<int>("FileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("file_id");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("file_name");

                    b.Property<string>("FileUrl")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("file_url");

                    b.Property<int>("ProposalId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("proposal_id");

                    b.Property<DateTime>("UploadedAt")
                        .HasColumnType("TEXT")
                        .HasColumnName("uploaded_at");

                    b.Property<int>("UploadedBy")
                        .HasColumnType("INTEGER")
                        .HasColumnName("uploaded_by");

                    b.HasKey("FileId");

                    b.ToTable("attachments", (string)null);
                });

            modelBuilder.Entity("FSSA.Models.Comment", b =>
                {
                    b.Property<int>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("comment_id");

                    b.Property<string>("CommentText")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("comment");

                    b.Property<int>("Commenter")
                        .HasColumnType("INTEGER")
                        .HasColumnName("commenter");

                    b.Property<int>("ReviewId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("review_id");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT")
                        .HasColumnName("timestamp");

                    b.HasKey("CommentId");

                    b.ToTable("comments", (string)null);
                });

            modelBuilder.Entity("FSSA.Models.EthicsReview", b =>
                {
                    b.Property<int>("EthicsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("ethics_id");

                    b.Property<bool>("EthicsRequired")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ethics_required");

                    b.Property<string>("HrecName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("hrec_name");

                    b.Property<string>("HrecNumber")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("hrec_number");

                    b.Property<string>("Opinion")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("opinion");

                    b.Property<int>("ProposalId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("proposal_id");

                    b.Property<DateTime>("ReviewDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("review_date");

                    b.Property<int>("ReviewedBy")
                        .HasColumnType("INTEGER")
                        .HasColumnName("reviewed_by");

                    b.HasKey("EthicsId");

                    b.ToTable("ethics_review", (string)null);
                });

            modelBuilder.Entity("FSSA.Models.Group", b =>
                {
                    b.Property<int>("GroupId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("group_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.HasKey("GroupId");

                    b.ToTable("groups", (string)null);
                });

            modelBuilder.Entity("FSSA.Models.Notification", b =>
                {
                    b.Property<int>("NotificationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("notification_id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT")
                        .HasColumnName("created_at");

                    b.Property<bool>("IsRead")
                        .HasColumnType("INTEGER")
                        .HasColumnName("is_read");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("message");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("user_id");

                    b.HasKey("NotificationId");

                    b.ToTable("notifications", (string)null);
                });

            modelBuilder.Entity("FSSA.Models.ProjectLevel", b =>
                {
                    b.Property<int>("LevelId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("level_id");

                    b.Property<string>("LevelName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("level_name");

                    b.HasKey("LevelId");

                    b.ToTable("project_levels", (string)null);
                });

            modelBuilder.Entity("FSSA.Models.Proposal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT")
                        .HasColumnName("created_at");

                    b.Property<DateTime>("EstimatedCompletionDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("estimated_completion_date");

                    b.Property<string>("EthicalConsiderations")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("ethical_considerations");

                    b.Property<string>("Method")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("method");

                    b.Property<string>("Milestones")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("milestones");

                    b.Property<string>("Outcomes")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("outcomes");

                    b.Property<int>("ProjectLevelId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("project_level_id");

                    b.Property<string>("Resources")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("resources");

                    b.Property<int>("StatusId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("status_id");

                    b.Property<int>("SubmittedBy")
                        .HasColumnType("INTEGER")
                        .HasColumnName("submitted_by");

                    b.Property<string>("Synopsis")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("synopsis");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("title");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.ToTable("proposals", (string)null);
                });

            modelBuilder.Entity("FSSA.Models.ProposalLog", b =>
                {
                    b.Property<int>("LogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("log_id");

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("action");

                    b.Property<int>("ChangedBy")
                        .HasColumnType("INTEGER")
                        .HasColumnName("changed_by");

                    b.Property<int>("ProposalId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("proposal_id");

                    b.Property<int>("StatusId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("status_id");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT")
                        .HasColumnName("timestamp");

                    b.HasKey("LogId");

                    b.ToTable("proposal_log", (string)null);
                });

            modelBuilder.Entity("FSSA.Models.Review", b =>
                {
                    b.Property<int>("ReviewId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("review_id");

                    b.Property<string>("Decision")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("decision");

                    b.Property<int>("ProposalId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("proposal_id");

                    b.Property<int>("ReviewedBy")
                        .HasColumnType("INTEGER")
                        .HasColumnName("reviewed_by");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT")
                        .HasColumnName("timestamp");

                    b.HasKey("ReviewId");

                    b.ToTable("reviews", (string)null);
                });

            modelBuilder.Entity("FSSA.Models.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("role_id");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("role_name");

                    b.HasKey("RoleId");

                    b.ToTable("roles", (string)null);
                });

            modelBuilder.Entity("FSSA.Models.Status", b =>
                {
                    b.Property<int>("StatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("status_id");

                    b.Property<string>("StatusName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("status_name");

                    b.HasKey("StatusId");

                    b.ToTable("status", (string)null);
                });

            modelBuilder.Entity("FSSA.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("user_id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT")
                        .HasColumnName("created_at");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("email");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("password_hash");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("role");

                    b.HasKey("UserId");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("FSSA.Models.UserGroup", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("user_id")
                        .HasColumnOrder(0);

                    b.Property<int>("GroupId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("group_id")
                        .HasColumnOrder(1);

                    b.Property<int>("RoleId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("role_id");

                    b.HasKey("UserId", "GroupId");

                    b.ToTable("user_groups", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
