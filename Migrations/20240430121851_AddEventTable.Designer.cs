﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PlayOfferService.Repositories;

#nullable disable

namespace PlayOfferService.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20240430121851_AddEventTable")]
    partial class AddEventTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("PlayOfferService.Domain.Events.BaseEvent<PlayOfferService.Domain.Events.IDomainEvent>", b =>
                {
                    b.Property<Guid>("EventId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("DomainEvent")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("EntityId")
                        .HasColumnType("char(36)");

                    b.Property<string>("EntityType")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("EventType")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("EventId");

                    b.ToTable("Events", (string)null);
                });

            modelBuilder.Entity("PlayOfferService.Models.Club", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.ToTable("Clubs");
                });

            modelBuilder.Entity("PlayOfferService.Models.Member", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("ClubId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("ClubId");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("PlayOfferService.Models.PlayOffer", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("AcceptedStartTime")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("ClubId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("CreatorId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("OpponentId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("ProposedEndTime")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("ProposedStartTime")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("ReservationId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("ClubId");

                    b.HasIndex("CreatorId");

                    b.HasIndex("OpponentId");

                    b.HasIndex("ReservationId");

                    b.ToTable("PlayOffers");
                });

            modelBuilder.Entity("PlayOfferService.Models.Reservation", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("char(36)");

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
