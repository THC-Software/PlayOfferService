using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayOfferService.Migrations
{
    /// <inheritdoc />
    public partial class memberExpansion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "playOffers",
                keyColumn: "id",
                keyValue: new Guid("c0f37db0-4c51-4bd3-86ab-48d6c78eb9c2"));

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "members",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "firstName",
                table: "members",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "lastName",
                table: "members",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "clubs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "clubs",
                keyColumn: "id",
                keyValue: new Guid("06b812a7-5131-4510-82ff-bffac33e0f3e"),
                column: "name",
                value: "Test Club");

            migrationBuilder.UpdateData(
                table: "members",
                keyColumn: "id",
                keyValue: new Guid("40c0981d-e2f8-4af3-ae6c-17f79f3ba8c2"),
                columns: new[] { "email", "firstName", "lastName" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "members",
                keyColumn: "id",
                keyValue: new Guid("ccc1c8fc-89b5-4026-b190-9d9e7e7bc18d"),
                columns: new[] { "email", "firstName", "lastName" },
                values: new object[] { null, null, null });

            migrationBuilder.InsertData(
                table: "playOffers",
                columns: new[] { "id", "acceptedStartTime", "clubId", "creatorId", "isCancelled", "opponentId", "proposedEndTime", "proposedStartTime", "reservationId" },
                values: new object[] { new Guid("7d079edb-d4c8-4e63-9c5b-35b80eaa69f6"), null, new Guid("06b812a7-5131-4510-82ff-bffac33e0f3e"), new Guid("40c0981d-e2f8-4af3-ae6c-17f79f3ba8c2"), false, null, new DateTime(2024, 6, 7, 12, 24, 44, 893, DateTimeKind.Utc).AddTicks(6493), new DateTime(2024, 6, 7, 11, 24, 44, 893, DateTimeKind.Utc).AddTicks(6491), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "playOffers",
                keyColumn: "id",
                keyValue: new Guid("7d079edb-d4c8-4e63-9c5b-35b80eaa69f6"));

            migrationBuilder.DropColumn(
                name: "email",
                table: "members");

            migrationBuilder.DropColumn(
                name: "firstName",
                table: "members");

            migrationBuilder.DropColumn(
                name: "lastName",
                table: "members");

            migrationBuilder.DropColumn(
                name: "name",
                table: "clubs");

            migrationBuilder.InsertData(
                table: "playOffers",
                columns: new[] { "id", "acceptedStartTime", "clubId", "creatorId", "isCancelled", "opponentId", "proposedEndTime", "proposedStartTime", "reservationId" },
                values: new object[] { new Guid("c0f37db0-4c51-4bd3-86ab-48d6c78eb9c2"), null, new Guid("06b812a7-5131-4510-82ff-bffac33e0f3e"), new Guid("40c0981d-e2f8-4af3-ae6c-17f79f3ba8c2"), false, null, new DateTime(2024, 6, 6, 19, 10, 3, 28, DateTimeKind.Utc).AddTicks(4637), new DateTime(2024, 6, 6, 18, 10, 3, 28, DateTimeKind.Utc).AddTicks(4634), null });
        }
    }
}
