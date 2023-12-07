﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using VakilPors.Data.Context;

#nullable disable

namespace VakilPors.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ChatUser", b =>
                {
                    b.Property<int>("ChatsId")
                        .HasColumnType("integer");

                    b.Property<int>("UsersId")
                        .HasColumnType("integer");

                    b.HasKey("ChatsId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("ChatUser");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.Chat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Chat");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.ChatMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CallStatus")
                        .HasColumnType("integer");

                    b.Property<int>("ChatId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsCall")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsEdited")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsFile")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsRead")
                        .HasColumnType("boolean");

                    b.Property<string>("Message")
                        .HasColumnType("text");

                    b.Property<int?>("ReplyId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("SendTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("SenderId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("ReplyId");

                    b.HasIndex("SenderId");

                    b.ToTable("ChatMessage");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.DocumentAccess", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("DocumentId")
                        .HasColumnType("integer");

                    b.Property<int>("DocumentStatus")
                        .HasColumnType("integer");

                    b.Property<int>("LawyerId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("DocumentId");

                    b.HasIndex("LawyerId");

                    b.ToTable("DocumentAccess");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.ForumThread", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("HasAnswer")
                        .HasColumnType("boolean");

                    b.Property<int>("LikeCount")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ForumThread");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.Lawyer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AboutMe")
                        .HasColumnType("text");

                    b.Property<string>("CallingCardImageUrl")
                        .HasColumnType("text");

                    b.Property<string>("City")
                        .HasColumnType("text");

                    b.Property<string>("Education")
                        .HasColumnType("text");

                    b.Property<string>("Gender")
                        .HasColumnType("text");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("boolean");

                    b.Property<string>("LicenseNumber")
                        .HasMaxLength(5)
                        .HasColumnType("character varying(5)");

                    b.Property<string>("MemberOf")
                        .HasColumnType("text");

                    b.Property<string>("NationalCardImageUrl")
                        .HasColumnType("text");

                    b.Property<string>("NationalCode")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<int>("NumberOfRates")
                        .HasColumnType("integer");

                    b.Property<string>("OfficeAddress")
                        .HasColumnType("text");

                    b.Property<string>("ProfileBackgroundPictureUrl")
                        .HasColumnType("text");

                    b.Property<string>("ProfileImageUrl")
                        .HasColumnType("text");

                    b.Property<double>("Rating")
                        .HasColumnType("double precision");

                    b.Property<string>("ResumeLink")
                        .HasColumnType("text");

                    b.Property<string>("Specialties")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<int>("Tokens")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int>("YearsOfExperience")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Lawyer");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.LegalDocument", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CaseName")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("DocumentCategory")
                        .HasColumnType("text");

                    b.Property<string>("FileUrl")
                        .HasColumnType("text");

                    b.Property<int>("MaximumBudget")
                        .HasColumnType("integer");

                    b.Property<int>("MinimumBudget")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("LegalDocument");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.Premium", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ServiceType")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Premium");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ServiceType = 0
                        },
                        new
                        {
                            Id = 2,
                            ServiceType = 1
                        },
                        new
                        {
                            Id = 3,
                            ServiceType = 2
                        },
                        new
                        {
                            Id = 4,
                            ServiceType = 3
                        });
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.Rate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("LawyerId")
                        .HasColumnType("integer");

                    b.Property<double>("RateNum")
                        .HasColumnType("double precision");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("LawyerId");

                    b.HasIndex("UserId");

                    b.ToTable("Rate");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ConcurrencyStamp = "8bdeb514-9677-4ab5-bcca-b7e097575597",
                            Name = "User",
                            NormalizedName = "USER"
                        },
                        new
                        {
                            Id = 2,
                            ConcurrencyStamp = "c7b51225-e9aa-4d81-8d75-b72266a01fde",
                            Name = "Vakil",
                            NormalizedName = "VAKIL"
                        },
                        new
                        {
                            Id = 3,
                            ConcurrencyStamp = "4e0f7985-9dbd-4d0f-9ff0-4420ee8fee75",
                            Name = "Admin",
                            NormalizedName = "ADMIN"
                        });
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.Subscribed", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ID"));

                    b.Property<DateTime>("ExpireDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("PremiumID")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.HasIndex("PremiumID");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Subscribed");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.ThreadComment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsSetAsAnswer")
                        .HasColumnType("boolean");

                    b.Property<int>("LikeCount")
                        .HasColumnType("integer");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ThreadId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ThreadId");

                    b.HasIndex("UserId");

                    b.ToTable("ThreadComment");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<string>("Authority")
                        .HasColumnType("text");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("IsIncome")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsPaid")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsSuccess")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsWithdraw")
                        .HasColumnType("boolean");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Transaction");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ActivationCode")
                        .HasColumnType("text");

                    b.Property<decimal>("Balance")
                        .HasColumnType("numeric");

                    b.Property<string>("Bio")
                        .HasColumnType("text");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("ForgetPasswordCode")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Job")
                        .HasColumnType("text");

                    b.Property<int?>("LawyerId")
                        .HasColumnType("integer");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("ProfileImageUrl")
                        .HasColumnType("text");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text");

                    b.Property<DateTime>("RefreshTokenExpiryTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<string>("Telegram")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.UserCommentLike", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CommentId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CommentId");

                    b.HasIndex("UserId");

                    b.ToTable("UserCommentLike");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.UserThreadLike", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ThreadId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ThreadId");

                    b.HasIndex("UserId");

                    b.ToTable("UserThreadLike");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.Visitor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("IPv4")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserGUID")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("VisitTime")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("UserGUID")
                        .IsUnique();

                    b.ToTable("Visitor");
                });

            modelBuilder.Entity("ChatUser", b =>
                {
                    b.HasOne("VakilPors.Core.Domain.Entities.Chat", null)
                        .WithMany()
                        .HasForeignKey("ChatsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VakilPors.Core.Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("VakilPors.Core.Domain.Entities.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("VakilPors.Core.Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("VakilPors.Core.Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.HasOne("VakilPors.Core.Domain.Entities.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VakilPors.Core.Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("VakilPors.Core.Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.ChatMessage", b =>
                {
                    b.HasOne("VakilPors.Core.Domain.Entities.Chat", "Chat")
                        .WithMany("ChatMessages")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VakilPors.Core.Domain.Entities.ChatMessage", "ReplyMessage")
                        .WithMany()
                        .HasForeignKey("ReplyId");

                    b.HasOne("VakilPors.Core.Domain.Entities.User", "Sender")
                        .WithMany("Messages")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("ReplyMessage");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.DocumentAccess", b =>
                {
                    b.HasOne("VakilPors.Core.Domain.Entities.LegalDocument", "Document")
                        .WithMany("Accesses")
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VakilPors.Core.Domain.Entities.Lawyer", "Lawyer")
                        .WithMany("DocumentAccesses")
                        .HasForeignKey("LawyerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Document");

                    b.Navigation("Lawyer");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.ForumThread", b =>
                {
                    b.HasOne("VakilPors.Core.Domain.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.Lawyer", b =>
                {
                    b.HasOne("VakilPors.Core.Domain.Entities.User", "User")
                        .WithOne("Lawyer")
                        .HasForeignKey("VakilPors.Core.Domain.Entities.Lawyer", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.LegalDocument", b =>
                {
                    b.HasOne("VakilPors.Core.Domain.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.Rate", b =>
                {
                    b.HasOne("VakilPors.Core.Domain.Entities.Lawyer", "Lawyer")
                        .WithMany()
                        .HasForeignKey("LawyerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VakilPors.Core.Domain.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Lawyer");

                    b.Navigation("User");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.Subscribed", b =>
                {
                    b.HasOne("VakilPors.Core.Domain.Entities.Premium", "Premium")
                        .WithMany()
                        .HasForeignKey("PremiumID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VakilPors.Core.Domain.Entities.User", "User")
                        .WithOne("Subscribed")
                        .HasForeignKey("VakilPors.Core.Domain.Entities.Subscribed", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Premium");

                    b.Navigation("User");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.ThreadComment", b =>
                {
                    b.HasOne("VakilPors.Core.Domain.Entities.ForumThread", "Thread")
                        .WithMany()
                        .HasForeignKey("ThreadId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VakilPors.Core.Domain.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Thread");

                    b.Navigation("User");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.Transaction", b =>
                {
                    b.HasOne("VakilPors.Core.Domain.Entities.User", "User")
                        .WithMany("Transactions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.UserCommentLike", b =>
                {
                    b.HasOne("VakilPors.Core.Domain.Entities.ThreadComment", "Comment")
                        .WithMany("UserLikes")
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VakilPors.Core.Domain.Entities.User", "User")
                        .WithMany("CommentLikes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Comment");

                    b.Navigation("User");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.UserThreadLike", b =>
                {
                    b.HasOne("VakilPors.Core.Domain.Entities.ForumThread", "Thread")
                        .WithMany("UserLikes")
                        .HasForeignKey("ThreadId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VakilPors.Core.Domain.Entities.User", "User")
                        .WithMany("ThreadLikes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Thread");

                    b.Navigation("User");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.Chat", b =>
                {
                    b.Navigation("ChatMessages");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.ForumThread", b =>
                {
                    b.Navigation("UserLikes");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.Lawyer", b =>
                {
                    b.Navigation("DocumentAccesses");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.LegalDocument", b =>
                {
                    b.Navigation("Accesses");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.ThreadComment", b =>
                {
                    b.Navigation("UserLikes");
                });

            modelBuilder.Entity("VakilPors.Core.Domain.Entities.User", b =>
                {
                    b.Navigation("CommentLikes");

                    b.Navigation("Lawyer");

                    b.Navigation("Messages");

                    b.Navigation("Subscribed");

                    b.Navigation("ThreadLikes");

                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
