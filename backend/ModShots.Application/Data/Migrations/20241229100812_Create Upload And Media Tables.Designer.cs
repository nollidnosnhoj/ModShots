﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ModShots.Application.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ModShots.Application.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241229100812_Create Upload And Media Tables")]
    partial class CreateUploadAndMediaTables
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ModShots.Domain.Media", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("character varying(26)")
                        .HasColumnName("id");

                    b.Property<string>("BlurHash")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("blur_hash");

                    b.Property<string>("Caption")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("caption");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)")
                        .HasColumnName("file_path");

                    b.Property<long>("FileSize")
                        .HasColumnType("bigint")
                        .HasColumnName("file_size");

                    b.Property<int>("Height")
                        .HasColumnType("integer")
                        .HasColumnName("height");

                    b.Property<bool>("IsComplete")
                        .HasColumnType("boolean")
                        .HasColumnName("is_complete");

                    b.Property<string>("Md5")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("md5");

                    b.Property<string>("MimeType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("mime_type");

                    b.Property<string>("OriginalFileName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("original_file_name");

                    b.Property<int?>("PostId")
                        .HasColumnType("integer")
                        .HasColumnName("post_id");

                    b.Property<int>("Width")
                        .HasColumnType("integer")
                        .HasColumnName("width");

                    b.HasKey("Id")
                        .HasName("pk_medias");

                    b.HasIndex("PostId")
                        .HasDatabaseName("ix_medias_post_id");

                    b.ToTable("medias", (string)null);
                });

            modelBuilder.Entity("ModShots.Domain.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("PublicId")
                        .IsRequired()
                        .HasMaxLength(12)
                        .HasColumnType("character varying(12)")
                        .HasColumnName("public_id");

                    b.Property<DateTimeOffset?>("PublishedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("published_at");

                    b.Property<int>("Severity")
                        .HasColumnType("integer")
                        .HasColumnName("severity");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.HasKey("Id")
                        .HasName("pk_posts");

                    b.HasIndex("PublicId")
                        .IsUnique()
                        .HasDatabaseName("ix_posts_public_id");

                    b.ToTable("posts", (string)null);
                });

            modelBuilder.Entity("ModShots.Domain.Media", b =>
                {
                    b.HasOne("ModShots.Domain.Post", null)
                        .WithMany("Medias")
                        .HasForeignKey("PostId")
                        .HasConstraintName("fk_medias_posts_post_id");
                });

            modelBuilder.Entity("ModShots.Domain.Post", b =>
                {
                    b.Navigation("Medias");
                });
#pragma warning restore 612, 618
        }
    }
}
