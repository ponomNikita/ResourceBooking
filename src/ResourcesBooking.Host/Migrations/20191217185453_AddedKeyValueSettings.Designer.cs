﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ResourcesBooking.Host;

namespace ResourcesBooking.Host.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20191217185453_AddedKeyValueSettings")]
    partial class AddedKeyValueSettings
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("ResourcesBooking.Host.Models.BookingTicket", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("BookedByLogin")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("BookingDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("BookingReason")
                        .HasColumnType("text");

                    b.Property<long>("DurationInMinutes")
                        .HasColumnType("bigint");

                    b.Property<Guid?>("ResourceId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("BookedByLogin");

                    b.HasIndex("ResourceId");

                    b.ToTable("BookingQueue");
                });

            modelBuilder.Entity("ResourcesBooking.Host.Models.KeyValue", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Key");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("ResourcesBooking.Host.Models.Resource", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("BookedByLogin")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset?>("BookedUntil")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("BookingReason")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("ResourcesGroupId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("BookedByLogin");

                    b.HasIndex("ResourcesGroupId");

                    b.ToTable("Resources");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Resource");
                });

            modelBuilder.Entity("ResourcesBooking.Host.Models.User", b =>
                {
                    b.Property<string>("Login")
                        .HasColumnType("text");

                    b.Property<string>("AvatarUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Login");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ResourcesBooking.Host.Models.ResourcesGroup", b =>
                {
                    b.HasBaseType("ResourcesBooking.Host.Models.Resource");

                    b.Property<bool>("AllowToBook")
                        .HasColumnType("boolean");

                    b.HasDiscriminator().HasValue("ResourcesGroup");
                });

            modelBuilder.Entity("ResourcesBooking.Host.Models.BookingTicket", b =>
                {
                    b.HasOne("ResourcesBooking.Host.Models.User", "BookedBy")
                        .WithMany()
                        .HasForeignKey("BookedByLogin");

                    b.HasOne("ResourcesBooking.Host.Models.Resource", null)
                        .WithMany("Queue")
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ResourcesBooking.Host.Models.Resource", b =>
                {
                    b.HasOne("ResourcesBooking.Host.Models.User", "BookedBy")
                        .WithMany()
                        .HasForeignKey("BookedByLogin");

                    b.HasOne("ResourcesBooking.Host.Models.ResourcesGroup", null)
                        .WithMany("Resources")
                        .HasForeignKey("ResourcesGroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
