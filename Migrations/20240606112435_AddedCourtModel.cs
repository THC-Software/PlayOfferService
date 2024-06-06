using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayOfferService.Migrations
{
    /// <inheritdoc />
    public partial class AddedCourtModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "playOffers",
                keyColumn: "id",
                keyValue: new Guid("781d0a5e-608d-4765-be03-4e9759fbf8be"));

            migrationBuilder.CreateTable(
                name: "courts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    clubId = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_courts", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "playOffers",
                columns: new[] { "id", "acceptedStartTime", "clubId", "creatorId", "isCancelled", "opponentId", "proposedEndTime", "proposedStartTime", "reservationId" },
                values: new object[] { new Guid("92ed1e90-1031-4999-8958-c6140cf4ea56"), null, new Guid("06b812a7-5131-4510-82ff-bffac33e0f3e"), new Guid("40c0981d-e2f8-4af3-ae6c-17f79f3ba8c2"), false, null, new DateTime(2024, 6, 6, 12, 24, 35, 25, DateTimeKind.Utc).AddTicks(9886), new DateTime(2024, 6, 6, 11, 24, 35, 25, DateTimeKind.Utc).AddTicks(9883), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "courts");

            migrationBuilder.DeleteData(
                table: "playOffers",
                keyColumn: "id",
                keyValue: new Guid("92ed1e90-1031-4999-8958-c6140cf4ea56"));

            migrationBuilder.InsertData(
                table: "playOffers",
                columns: new[] { "id", "acceptedStartTime", "clubId", "creatorId", "isCancelled", "opponentId", "proposedEndTime", "proposedStartTime", "reservationId" },
                values: new object[] { new Guid("781d0a5e-608d-4765-be03-4e9759fbf8be"), null, new Guid("06b812a7-5131-4510-82ff-bffac33e0f3e"), new Guid("40c0981d-e2f8-4af3-ae6c-17f79f3ba8c2"), false, null, new DateTime(2024, 6, 6, 9, 59, 5, 597, DateTimeKind.Utc).AddTicks(5737), new DateTime(2024, 6, 6, 8, 59, 5, 597, DateTimeKind.Utc).AddTicks(5734), null });
        }
    }
}
