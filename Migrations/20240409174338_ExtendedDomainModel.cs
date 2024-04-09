using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayOfferService.Migrations
{
    /// <inheritdoc />
    public partial class ExtendedDomainModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PlayDate",
                table: "PlayOffers",
                newName: "ProposedStartTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedStartTime",
                table: "PlayOffers",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ProposedEndTime",
                table: "PlayOffers",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Clubs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clubs", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClubId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Members_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Clubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PlayOffers_ClubId",
                table: "PlayOffers",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayOffers_CreatorId",
                table: "PlayOffers",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayOffers_OpponentId",
                table: "PlayOffers",
                column: "OpponentId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayOffers_ReservationId",
                table: "PlayOffers",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_ClubId",
                table: "Members",
                column: "ClubId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayOffers_Clubs_ClubId",
                table: "PlayOffers",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayOffers_Members_CreatorId",
                table: "PlayOffers",
                column: "CreatorId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayOffers_Members_OpponentId",
                table: "PlayOffers",
                column: "OpponentId",
                principalTable: "Members",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayOffers_Reservations_ReservationId",
                table: "PlayOffers",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayOffers_Clubs_ClubId",
                table: "PlayOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayOffers_Members_CreatorId",
                table: "PlayOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayOffers_Members_OpponentId",
                table: "PlayOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayOffers_Reservations_ReservationId",
                table: "PlayOffers");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Clubs");

            migrationBuilder.DropIndex(
                name: "IX_PlayOffers_ClubId",
                table: "PlayOffers");

            migrationBuilder.DropIndex(
                name: "IX_PlayOffers_CreatorId",
                table: "PlayOffers");

            migrationBuilder.DropIndex(
                name: "IX_PlayOffers_OpponentId",
                table: "PlayOffers");

            migrationBuilder.DropIndex(
                name: "IX_PlayOffers_ReservationId",
                table: "PlayOffers");

            migrationBuilder.DropColumn(
                name: "AcceptedStartTime",
                table: "PlayOffers");

            migrationBuilder.DropColumn(
                name: "ProposedEndTime",
                table: "PlayOffers");

            migrationBuilder.RenameColumn(
                name: "ProposedStartTime",
                table: "PlayOffers",
                newName: "PlayDate");
        }
    }
}
