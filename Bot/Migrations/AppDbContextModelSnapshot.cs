﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RatingsBot.Data;

namespace RatingsBot.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasPostgresExtension("uuid-ossp")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.9")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("RatingsBot.Models.Category", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("RatingsBot.Models.Item", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<string>("CategoryId1")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId1");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("RatingsBot.Models.Rating", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<int>("ItemId")
                        .HasColumnType("integer");

                    b.Property<string>("ItemId1")
                        .HasColumnType("text");

                    b.Property<long>("UsedId")
                        .HasColumnType("bigint");

                    b.Property<long?>("UserId")
                        .HasColumnType("bigint");

                    b.Property<int>("Value")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ItemId1");

                    b.HasIndex("UserId");

                    b.ToTable("Ratings");
                });

            modelBuilder.Entity("RatingsBot.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("RatingsBot.Models.Item", b =>
                {
                    b.HasOne("RatingsBot.Models.Category", "Category")
                        .WithMany("Items")
                        .HasForeignKey("CategoryId1");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("RatingsBot.Models.Rating", b =>
                {
                    b.HasOne("RatingsBot.Models.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId1");

                    b.HasOne("RatingsBot.Models.User", "User")
                        .WithMany("Ratings")
                        .HasForeignKey("UserId");

                    b.Navigation("Item");

                    b.Navigation("User");
                });

            modelBuilder.Entity("RatingsBot.Models.Category", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("RatingsBot.Models.User", b =>
                {
                    b.Navigation("Ratings");
                });
#pragma warning restore 612, 618
        }
    }
}
