using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayOfferService.Migrations
{
    /// <inheritdoc />
    public partial class ReservationChangedCourtId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "playOffers",
                keyColumn: "id",
                keyValue: new Guid("7d079edb-d4c8-4e63-9c5b-35b80eaa69f6"));

            migrationBuilder.DropColumn(
                name: "courtIds",
                table: "reservations");

            migrationBuilder.AddColumn<Guid>(
                name: "courtId",
                table: "reservations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "lastName",
                table: "members",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "firstName",
                table: "members",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "members",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "members",
                keyColumn: "id",
                keyValue: new Guid("40c0981d-e2f8-4af3-ae6c-17f79f3ba8c2"),
                columns: new[] { "email", "firstName", "lastName" },
                values: new object[] { "hans@müller.de", "Hans", "Müller" });

            migrationBuilder.UpdateData(
                table: "members",
                keyColumn: "id",
                keyValue: new Guid("ccc1c8fc-89b5-4026-b190-9d9e7e7bc18d"),
                columns: new[] { "email", "firstName", "lastName" },
                values: new object[] { "friedrich@bäcker.at", "Friedrich", "Bäcker" });

            migrationBuilder.InsertData(
                table: "playOffers",
                columns: new[] { "id", "acceptedStartTime", "clubId", "creatorId", "isCancelled", "opponentId", "proposedEndTime", "proposedStartTime", "reservationId" },
                values: new object[] { new Guid("a4e3d4e5-5215-4094-a333-ce3ee8154675"), null, new Guid("06b812a7-5131-4510-82ff-bffac33e0f3e"), new Guid("40c0981d-e2f8-4af3-ae6c-17f79f3ba8c2"), false, null, new DateTime(2024, 6, 22, 10, 27, 32, 806, DateTimeKind.Utc).AddTicks(9696), new DateTime(2024, 6, 22, 9, 27, 32, 806, DateTimeKind.Utc).AddTicks(9693), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "playOffers",
                keyColumn: "id",
                keyValue: new Guid("a4e3d4e5-5215-4094-a333-ce3ee8154675"));

            migrationBuilder.DropColumn(
                name: "courtId",
                table: "reservations");

            migrationBuilder.AddColumn<List<Guid>>(
                name: "courtIds",
                table: "reservations",
                type: "uuid[]",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "lastName",
                table: "members",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "firstName",
                table: "members",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "members",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

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
    }
}
