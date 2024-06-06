using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayOfferService.Migrations
{
    /// <inheritdoc />
    public partial class ExpandedReservationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "playOffers",
                keyColumn: "id",
                keyValue: new Guid("6fdd5f06-9099-4e7c-ad94-e184df54676b"));

            migrationBuilder.AddColumn<List<Guid>>(
                name: "courtIds",
                table: "reservations",
                type: "uuid[]",
                nullable: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "endTime",
                table: "reservations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "isCancelled",
                table: "reservations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<List<Guid>>(
                name: "participantsIds",
                table: "reservations",
                type: "uuid[]",
                nullable: false);

            migrationBuilder.AddColumn<Guid>(
                name: "reservantId",
                table: "reservations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "startTime",
                table: "reservations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "playOffers",
                columns: new[] { "id", "acceptedStartTime", "clubId", "creatorId", "isCancelled", "opponentId", "proposedEndTime", "proposedStartTime", "reservationId" },
                values: new object[] { new Guid("781d0a5e-608d-4765-be03-4e9759fbf8be"), null, new Guid("06b812a7-5131-4510-82ff-bffac33e0f3e"), new Guid("40c0981d-e2f8-4af3-ae6c-17f79f3ba8c2"), false, null, new DateTime(2024, 6, 6, 9, 59, 5, 597, DateTimeKind.Utc).AddTicks(5737), new DateTime(2024, 6, 6, 8, 59, 5, 597, DateTimeKind.Utc).AddTicks(5734), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "playOffers",
                keyColumn: "id",
                keyValue: new Guid("781d0a5e-608d-4765-be03-4e9759fbf8be"));

            migrationBuilder.DropColumn(
                name: "courtIds",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "endTime",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "isCancelled",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "participantsIds",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "reservantId",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "startTime",
                table: "reservations");

            migrationBuilder.InsertData(
                table: "playOffers",
                columns: new[] { "id", "acceptedStartTime", "clubId", "creatorId", "isCancelled", "opponentId", "proposedEndTime", "proposedStartTime", "reservationId" },
                values: new object[] { new Guid("6fdd5f06-9099-4e7c-ad94-e184df54676b"), null, new Guid("06b812a7-5131-4510-82ff-bffac33e0f3e"), new Guid("40c0981d-e2f8-4af3-ae6c-17f79f3ba8c2"), false, null, new DateTime(2024, 6, 2, 12, 39, 23, 865, DateTimeKind.Utc).AddTicks(7636), new DateTime(2024, 6, 2, 11, 39, 23, 865, DateTimeKind.Utc).AddTicks(7634), null });
        }
    }
}
