using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PlayOfferService.Migrations.DbRead
{
    /// <inheritdoc />
    public partial class RefactorIsLockedToStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clubs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_clubs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    eventId = table.Column<Guid>(type: "uuid", nullable: false),
                    entityId = table.Column<Guid>(type: "uuid", nullable: false),
                    eventType = table.Column<string>(type: "text", nullable: false),
                    entityType = table.Column<string>(type: "text", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    eventData = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_events", x => x.eventId);
                });

            migrationBuilder.CreateTable(
                name: "members",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    clubId = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_members", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "reservations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_reservations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "playOffers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    clubId = table.Column<Guid>(type: "uuid", nullable: false),
                    creatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    opponentId = table.Column<Guid>(type: "uuid", nullable: true),
                    proposedStartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    proposedEndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    acceptedStartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    reservationId = table.Column<Guid>(type: "uuid", nullable: true),
                    isCancelled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_playOffers", x => x.id);
                    table.ForeignKey(
                        name: "fK_playOffers_clubs_clubId",
                        column: x => x.clubId,
                        principalTable: "clubs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fK_playOffers_members_creatorId",
                        column: x => x.creatorId,
                        principalTable: "members",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fK_playOffers_members_opponentId",
                        column: x => x.opponentId,
                        principalTable: "members",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fK_playOffers_reservations_reservationId",
                        column: x => x.reservationId,
                        principalTable: "reservations",
                        principalColumn: "id");
                });

            migrationBuilder.InsertData(
                table: "clubs",
                columns: new[] { "id", "status" },
                values: new object[] { new Guid("06b812a7-5131-4510-82ff-bffac33e0f3e"), 0 });

            migrationBuilder.InsertData(
                table: "members",
                columns: new[] { "id", "clubId", "status" },
                values: new object[,]
                {
                    { new Guid("40c0981d-e2f8-4af3-ae6c-17f79f3ba8c2"), new Guid("06b812a7-5131-4510-82ff-bffac33e0f3e"), 0 },
                    { new Guid("ccc1c8fc-89b5-4026-b190-9d9e7e7bc18d"), new Guid("06b812a7-5131-4510-82ff-bffac33e0f3e"), 0 }
                });

            migrationBuilder.InsertData(
                table: "playOffers",
                columns: new[] { "id", "acceptedStartTime", "clubId", "creatorId", "isCancelled", "opponentId", "proposedEndTime", "proposedStartTime", "reservationId" },
                values: new object[] { new Guid("3655e6cd-5d91-44f9-8b0c-9548a06b6762"), null, new Guid("06b812a7-5131-4510-82ff-bffac33e0f3e"), new Guid("40c0981d-e2f8-4af3-ae6c-17f79f3ba8c2"), false, null, new DateTime(2024, 5, 29, 8, 57, 31, 906, DateTimeKind.Utc).AddTicks(4438), new DateTime(2024, 5, 29, 7, 57, 31, 906, DateTimeKind.Utc).AddTicks(4436), null });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "playOffers");

            migrationBuilder.DropTable(
                name: "clubs");

            migrationBuilder.DropTable(
                name: "members");

            migrationBuilder.DropTable(
                name: "reservations");
        }
    }
}
