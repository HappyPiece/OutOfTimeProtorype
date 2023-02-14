using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OutOfTimePrototype.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CampusBuildings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampusBuildings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClassTypes",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassTypes", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Clusters",
                columns: table => new
                {
                    Number = table.Column<string>(type: "text", nullable: false),
                    SuperClusterNumber = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clusters", x => x.Number);
                    table.ForeignKey(
                        name: "FK_Clusters_Clusters_SuperClusterNumber",
                        column: x => x.SuperClusterNumber,
                        principalTable: "Clusters",
                        principalColumn: "Number");
                });

            migrationBuilder.CreateTable(
                name: "Educators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    MiddleName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Educators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimeSlots",
                columns: table => new
                {
                    Number = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSlots", x => x.Number);
                });

            migrationBuilder.CreateTable(
                name: "LectureHalls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    HostBuildingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LectureHalls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LectureHalls_CampusBuildings_HostBuildingId",
                        column: x => x.HostBuildingId,
                        principalTable: "CampusBuildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TypeName = table.Column<string>(type: "text", nullable: true),
                    TimeSlotNumber = table.Column<int>(type: "integer", nullable: false),
                    ClusterNumber = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LectureHallId = table.Column<Guid>(type: "uuid", nullable: true),
                    EducatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Classes_ClassTypes_TypeName",
                        column: x => x.TypeName,
                        principalTable: "ClassTypes",
                        principalColumn: "Name");
                    table.ForeignKey(
                        name: "FK_Classes_Clusters_ClusterNumber",
                        column: x => x.ClusterNumber,
                        principalTable: "Clusters",
                        principalColumn: "Number",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Classes_Educators_EducatorId",
                        column: x => x.EducatorId,
                        principalTable: "Educators",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Classes_LectureHalls_LectureHallId",
                        column: x => x.LectureHallId,
                        principalTable: "LectureHalls",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Classes_TimeSlots_TimeSlotNumber",
                        column: x => x.TimeSlotNumber,
                        principalTable: "TimeSlots",
                        principalColumn: "Number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ClassTypes",
                column: "Name",
                values: new object[]
                {
                    "Exam",
                    "Laboratory",
                    "Lecture",
                    "Practice",
                    "Seminar"
                });

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "Number", "EndTime", "StartTime" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 2, 14, 3, 20, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 14, 1, 45, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2023, 2, 14, 5, 10, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 14, 3, 35, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2023, 2, 14, 7, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 14, 5, 25, 0, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2023, 2, 14, 9, 20, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 14, 7, 45, 0, 0, DateTimeKind.Utc) },
                    { 5, new DateTime(2023, 2, 14, 11, 10, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 14, 9, 35, 0, 0, DateTimeKind.Utc) },
                    { 6, new DateTime(2023, 2, 14, 13, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 14, 11, 25, 0, 0, DateTimeKind.Utc) },
                    { 7, new DateTime(2023, 2, 14, 14, 50, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 14, 13, 15, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Classes_ClusterNumber",
                table: "Classes",
                column: "ClusterNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_EducatorId",
                table: "Classes",
                column: "EducatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_LectureHallId",
                table: "Classes",
                column: "LectureHallId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_TimeSlotNumber",
                table: "Classes",
                column: "TimeSlotNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_TypeName",
                table: "Classes",
                column: "TypeName");

            migrationBuilder.CreateIndex(
                name: "IX_Clusters_SuperClusterNumber",
                table: "Clusters",
                column: "SuperClusterNumber");

            migrationBuilder.CreateIndex(
                name: "IX_LectureHalls_HostBuildingId",
                table: "LectureHalls",
                column: "HostBuildingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "ClassTypes");

            migrationBuilder.DropTable(
                name: "Clusters");

            migrationBuilder.DropTable(
                name: "Educators");

            migrationBuilder.DropTable(
                name: "LectureHalls");

            migrationBuilder.DropTable(
                name: "TimeSlots");

            migrationBuilder.DropTable(
                name: "CampusBuildings");
        }
    }
}
