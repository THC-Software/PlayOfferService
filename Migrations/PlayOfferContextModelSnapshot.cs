﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PlayOfferService.Repositories;

#nullable disable

namespace PlayOfferService.Migrations
{
    [DbContext(typeof(PlayOfferContext))]
    partial class PlayOfferContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("PlayOfferService.Models.Club", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Clubs");
                });

            modelBuilder.Entity("PlayOfferService.Models.Member", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("ClubId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ClubId");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("PlayOfferService.Models.PlayOffer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("AcceptedStartTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("ClubId")
                        .HasColumnType("int");

                    b.Property<int>("CreatorId")
                        .HasColumnType("int");

                    b.Property<int?>("OpponentId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ProposedEndTime")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("ProposedStartTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("ReservationId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ClubId");

                    b.HasIndex("CreatorId");

                    b.HasIndex("OpponentId");

                    b.HasIndex("ReservationId");

                    b.ToTable("PlayOffers");
                });

            modelBuilder.Entity("PlayOfferService.Models.Reservation", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("PlayOfferService.Models.Member", b =>
                {
                    b.HasOne("PlayOfferService.Models.Club", "Club")
                        .WithMany()
                        .HasForeignKey("ClubId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Club");
                });

            modelBuilder.Entity("PlayOfferService.Models.PlayOffer", b =>
                {
                    b.HasOne("PlayOfferService.Models.Club", "Club")
                        .WithMany()
                        .HasForeignKey("ClubId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PlayOfferService.Models.Member", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PlayOfferService.Models.Member", "Opponent")
                        .WithMany()
                        .HasForeignKey("OpponentId");

                    b.HasOne("PlayOfferService.Models.Reservation", "Reservation")
                        .WithMany()
                        .HasForeignKey("ReservationId");

                    b.Navigation("Club");

                    b.Navigation("Creator");

                    b.Navigation("Opponent");

                    b.Navigation("Reservation");
                });
#pragma warning restore 612, 618
        }
    }
}
