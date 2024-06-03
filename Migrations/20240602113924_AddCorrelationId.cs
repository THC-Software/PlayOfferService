using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayOfferService.Migrations
{
    /// <inheritdoc />
    public partial class AddCorrelationId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "playOffers",
                keyColumn: "id",
                keyValue: new Guid("ec8af52d-49b5-458b-baec-ec85d048ab58"));

            migrationBuilder.AddColumn<Guid>(
                name: "correlationId",
                table: "events",
                type: "uuid",
                nullable: true);

            migrationBuilder.InsertData(
                table: "playOffers",
                columns: new[] { "id", "acceptedStartTime", "clubId", "creatorId", "isCancelled", "opponentId", "proposedEndTime", "proposedStartTime", "reservationId" },
                values: new object[] { new Guid("6fdd5f06-9099-4e7c-ad94-e184df54676b"), null, new Guid("06b812a7-5131-4510-82ff-bffac33e0f3e"), new Guid("40c0981d-e2f8-4af3-ae6c-17f79f3ba8c2"), false, null, new DateTime(2024, 6, 2, 12, 39, 23, 865, DateTimeKind.Utc).AddTicks(7636), new DateTime(2024, 6, 2, 11, 39, 23, 865, DateTimeKind.Utc).AddTicks(7634), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "playOffers",
                keyColumn: "id",
                keyValue: new Guid("6fdd5f06-9099-4e7c-ad94-e184df54676b"));

            migrationBuilder.DropColumn(
                name: "correlationId",
                table: "events");

            migrationBuilder.InsertData(
                table: "playOffers",
                columns: new[] { "id", "acceptedStartTime", "clubId", "creatorId", "isCancelled", "opponentId", "proposedEndTime", "proposedStartTime", "reservationId" },
                values: new object[] { new Guid("ec8af52d-49b5-458b-baec-ec85d048ab58"), null, new Guid("06b812a7-5131-4510-82ff-bffac33e0f3e"), new Guid("40c0981d-e2f8-4af3-ae6c-17f79f3ba8c2"), false, null, new DateTime(2024, 6, 2, 12, 15, 25, 414, DateTimeKind.Utc).AddTicks(639), new DateTime(2024, 6, 2, 11, 15, 25, 414, DateTimeKind.Utc).AddTicks(635), null });
        }
    }
}
