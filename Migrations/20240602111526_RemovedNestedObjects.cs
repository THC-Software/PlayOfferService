using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayOfferService.Migrations
{
    /// <inheritdoc />
    public partial class RemovedNestedObjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fK_playOffers_clubs_clubId",
                table: "playOffers");

            migrationBuilder.DropForeignKey(
                name: "fK_playOffers_members_creatorId",
                table: "playOffers");

            migrationBuilder.DropForeignKey(
                name: "fK_playOffers_members_opponentId",
                table: "playOffers");

            migrationBuilder.DropForeignKey(
                name: "fK_playOffers_reservations_reservationId",
                table: "playOffers");

            migrationBuilder.DropIndex(
                name: "iX_playOffers_clubId",
                table: "playOffers");

            migrationBuilder.DropIndex(
                name: "iX_playOffers_creatorId",
                table: "playOffers");

            migrationBuilder.DropIndex(
                name: "iX_playOffers_opponentId",
                table: "playOffers");

            migrationBuilder.DropIndex(
                name: "iX_playOffers_reservationId",
                table: "playOffers");

            migrationBuilder.DeleteData(
                table: "playOffers",
                keyColumn: "id",
                keyValue: new Guid("bd18fca2-8708-495a-8e15-633fa33e8a7b"));

            migrationBuilder.InsertData(
                table: "playOffers",
                columns: new[] { "id", "acceptedStartTime", "clubId", "creatorId", "isCancelled", "opponentId", "proposedEndTime", "proposedStartTime", "reservationId" },
                values: new object[] { new Guid("ec8af52d-49b5-458b-baec-ec85d048ab58"), null, new Guid("06b812a7-5131-4510-82ff-bffac33e0f3e"), new Guid("40c0981d-e2f8-4af3-ae6c-17f79f3ba8c2"), false, null, new DateTime(2024, 6, 2, 12, 15, 25, 414, DateTimeKind.Utc).AddTicks(639), new DateTime(2024, 6, 2, 11, 15, 25, 414, DateTimeKind.Utc).AddTicks(635), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "playOffers",
                keyColumn: "id",
                keyValue: new Guid("ec8af52d-49b5-458b-baec-ec85d048ab58"));

            migrationBuilder.InsertData(
                table: "playOffers",
                columns: new[] { "id", "acceptedStartTime", "clubId", "creatorId", "isCancelled", "opponentId", "proposedEndTime", "proposedStartTime", "reservationId" },
                values: new object[] { new Guid("bd18fca2-8708-495a-8e15-633fa33e8a7b"), null, new Guid("06b812a7-5131-4510-82ff-bffac33e0f3e"), new Guid("40c0981d-e2f8-4af3-ae6c-17f79f3ba8c2"), false, null, new DateTime(2024, 6, 1, 14, 28, 22, 322, DateTimeKind.Utc).AddTicks(9668), new DateTime(2024, 6, 1, 13, 28, 22, 322, DateTimeKind.Utc).AddTicks(9665), null });

            migrationBuilder.CreateIndex(
                name: "iX_playOffers_clubId",
                table: "playOffers",
                column: "clubId");

            migrationBuilder.CreateIndex(
                name: "iX_playOffers_creatorId",
                table: "playOffers",
                column: "creatorId");

            migrationBuilder.CreateIndex(
                name: "iX_playOffers_opponentId",
                table: "playOffers",
                column: "opponentId");

            migrationBuilder.CreateIndex(
                name: "iX_playOffers_reservationId",
                table: "playOffers",
                column: "reservationId");

            migrationBuilder.AddForeignKey(
                name: "fK_playOffers_clubs_clubId",
                table: "playOffers",
                column: "clubId",
                principalTable: "clubs",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fK_playOffers_members_creatorId",
                table: "playOffers",
                column: "creatorId",
                principalTable: "members",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fK_playOffers_members_opponentId",
                table: "playOffers",
                column: "opponentId",
                principalTable: "members",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fK_playOffers_reservations_reservationId",
                table: "playOffers",
                column: "reservationId",
                principalTable: "reservations",
                principalColumn: "id");
        }
    }
}
