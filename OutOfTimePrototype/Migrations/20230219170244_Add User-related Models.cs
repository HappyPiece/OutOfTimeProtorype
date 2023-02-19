using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OutOfTimePrototype.Migrations
{
    /// <inheritdoc />
    public partial class AddUserrelatedModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Educators",
                keyColumn: "Id",
                keyValue: new Guid("03ac257d-0977-44b0-a49c-9d03f976db79"));

            migrationBuilder.DeleteData(
                table: "Educators",
                keyColumn: "Id",
                keyValue: new Guid("209f16f1-3030-47bf-b4e4-1b88cd9a0733"));

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    ClaimedRoles = table.Column<int[]>(type: "integer[]", nullable: false),
                    VerifiedRoles = table.Column<int[]>(type: "integer[]", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    MiddleName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    ScheduleSelfId = table.Column<Guid>(type: "uuid", nullable: true),
                    GradeBookNumber = table.Column<string>(type: "text", nullable: true),
                    ClusterNumber = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Clusters_ClusterNumber",
                        column: x => x.ClusterNumber,
                        principalTable: "Clusters",
                        principalColumn: "Number");
                    table.ForeignKey(
                        name: "FK_Users_Educators_ScheduleSelfId",
                        column: x => x.ScheduleSelfId,
                        principalTable: "Educators",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Educators",
                columns: new[] { "Id", "FirstName", "LastName", "MiddleName" },
                values: new object[,]
                {
                    { new Guid("4570a206-d5f0-4b76-85a1-ed13a6fe58f2"), "Prepod", "Prepodov", "Prepodovich" },
                    { new Guid("bf5d33dd-6ccd-436f-a26d-d4ab81c1c11e"), "Educator", "Educatorov", "Educatorovich" }
                });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 1,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 20, 3, 20, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 20, 1, 45, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 2,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 20, 5, 10, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 20, 3, 35, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 3,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 20, 7, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 20, 5, 25, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 4,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 20, 9, 20, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 20, 7, 45, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 5,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 20, 11, 10, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 20, 9, 35, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 6,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 20, 13, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 20, 11, 25, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 7,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 20, 14, 50, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 20, 13, 15, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.CreateIndex(
                name: "IX_Users_ClusterNumber",
                table: "Users",
                column: "ClusterNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ScheduleSelfId",
                table: "Users",
                column: "ScheduleSelfId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DeleteData(
                table: "Educators",
                keyColumn: "Id",
                keyValue: new Guid("4570a206-d5f0-4b76-85a1-ed13a6fe58f2"));

            migrationBuilder.DeleteData(
                table: "Educators",
                keyColumn: "Id",
                keyValue: new Guid("bf5d33dd-6ccd-436f-a26d-d4ab81c1c11e"));

            migrationBuilder.InsertData(
                table: "Educators",
                columns: new[] { "Id", "FirstName", "LastName", "MiddleName" },
                values: new object[,]
                {
                    { new Guid("03ac257d-0977-44b0-a49c-9d03f976db79"), "Prepod", "Prepodov", "Prepodovich" },
                    { new Guid("209f16f1-3030-47bf-b4e4-1b88cd9a0733"), "Educator", "Educatorov", "Educatorovich" }
                });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 1,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 15, 3, 20, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 15, 1, 45, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 2,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 15, 5, 10, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 15, 3, 35, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 3,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 15, 7, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 15, 5, 25, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 4,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 15, 9, 20, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 15, 7, 45, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 5,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 15, 11, 10, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 15, 9, 35, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 6,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 15, 13, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 15, 11, 25, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 7,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 15, 14, 50, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 15, 13, 15, 0, 0, DateTimeKind.Utc) });
        }
    }
}
