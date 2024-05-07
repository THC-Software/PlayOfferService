using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayOfferService.Migrations
{
    /// <inheritdoc />
    public partial class ChangedNamingConventionToCamelCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "clubs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_clubs", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    eventId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    entityId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    eventType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    entityType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    eventData = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_Events", x => x.eventId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "reservations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_reservations", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "members",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    clubId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_members", x => x.id);
                    table.ForeignKey(
                        name: "fK_members_clubs_clubId",
                        column: x => x.clubId,
                        principalTable: "clubs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "playOffers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    clubId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    creatorId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    opponentId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    proposedStartTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    proposedEndTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    acceptedStartTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    reservationId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "iX_members_clubId",
                table: "members",
                column: "clubId");

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
                name: "Events");

            migrationBuilder.DropTable(
                name: "playOffers");

            migrationBuilder.DropTable(
                name: "members");

            migrationBuilder.DropTable(
                name: "reservations");

            migrationBuilder.DropTable(
                name: "clubs");
        }
    }
}
