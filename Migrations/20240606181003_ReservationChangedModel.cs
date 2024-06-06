using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayOfferService.Migrations
{
    /// <inheritdoc />
    public partial class ReservationChangedModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "playOffers",
                keyColumn: "id",
                keyValue: new Guid("92ed1e90-1031-4999-8958-c6140cf4ea56"));

            migrationBuilder.AlterColumn<List<Guid>>(
                name: "participantsIds",
                table: "reservations",
                type: "uuid[]",
                nullable: true,
                oldClrType: typeof(List<Guid>),
                oldType: "uuid[]");

            migrationBuilder.InsertData(
                table: "playOffers",
                columns: new[] { "id", "acceptedStartTime", "clubId", "creatorId", "isCancelled", "opponentId", "proposedEndTime", "proposedStartTime", "reservationId" },
                values: new object[] { new Guid("c0f37db0-4c51-4bd3-86ab-48d6c78eb9c2"), null, new Guid("06b812a7-5131-4510-82ff-bffac33e0f3e"), new Guid("40c0981d-e2f8-4af3-ae6c-17f79f3ba8c2"), false, null, new DateTime(2024, 6, 6, 19, 10, 3, 28, DateTimeKind.Utc).AddTicks(4637), new DateTime(2024, 6, 6, 18, 10, 3, 28, DateTimeKind.Utc).AddTicks(4634), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "playOffers",
                keyColumn: "id",
                keyValue: new Guid("c0f37db0-4c51-4bd3-86ab-48d6c78eb9c2"));

            migrationBuilder.AlterColumn<List<Guid>>(
                name: "participantsIds",
                table: "reservations",
                type: "uuid[]",
                nullable: false,
                oldClrType: typeof(List<Guid>),
                oldType: "uuid[]",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "playOffers",
                columns: new[] { "id", "acceptedStartTime", "clubId", "creatorId", "isCancelled", "opponentId", "proposedEndTime", "proposedStartTime", "reservationId" },
                values: new object[] { new Guid("92ed1e90-1031-4999-8958-c6140cf4ea56"), null, new Guid("06b812a7-5131-4510-82ff-bffac33e0f3e"), new Guid("40c0981d-e2f8-4af3-ae6c-17f79f3ba8c2"), false, null, new DateTime(2024, 6, 6, 12, 24, 35, 25, DateTimeKind.Utc).AddTicks(9886), new DateTime(2024, 6, 6, 11, 24, 35, 25, DateTimeKind.Utc).AddTicks(9883), null });
        }
    }
}
