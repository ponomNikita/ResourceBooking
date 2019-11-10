using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ResourcesBooking.Host.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Login = table.Column<string>(nullable: false),
                    AvatarUrl = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Login);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    BookedUntil = table.Column<DateTimeOffset>(nullable: true),
                    BookingReason = table.Column<string>(nullable: true),
                    BookedByLogin = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    ResourcesGroupId = table.Column<Guid>(nullable: true),
                    AllowToBook = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resources_Users_BookedByLogin",
                        column: x => x.BookedByLogin,
                        principalTable: "Users",
                        principalColumn: "Login",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Resources_Resources_ResourcesGroupId",
                        column: x => x.ResourcesGroupId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingQueue",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BookingReason = table.Column<string>(nullable: true),
                    BookedByLogin = table.Column<string>(nullable: true),
                    DurationInMinutes = table.Column<long>(nullable: false),
                    BookingDate = table.Column<DateTimeOffset>(nullable: false),
                    ResourceId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingQueue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingQueue_Users_BookedByLogin",
                        column: x => x.BookedByLogin,
                        principalTable: "Users",
                        principalColumn: "Login",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingQueue_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingQueue_BookedByLogin",
                table: "BookingQueue",
                column: "BookedByLogin");

            migrationBuilder.CreateIndex(
                name: "IX_BookingQueue_ResourceId",
                table: "BookingQueue",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_BookedByLogin",
                table: "Resources",
                column: "BookedByLogin");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_ResourcesGroupId",
                table: "Resources",
                column: "ResourcesGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingQueue");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
