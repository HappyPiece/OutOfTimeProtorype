using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OutOfTimePrototype.Migrations
{
    /// <inheritdoc />
    public partial class AddRootUserSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Clusters",
                keyColumn: "Number",
                keyValue: "9721");

            migrationBuilder.DeleteData(
                table: "Educators",
                keyColumn: "Id",
                keyValue: new Guid("878b71be-0f64-4893-a75a-1dd0205609d5"));

            migrationBuilder.DeleteData(
                table: "Educators",
                keyColumn: "Id",
                keyValue: new Guid("f718d1e0-f6ce-4c16-ba28-00ec621f0cce"));

            migrationBuilder.InsertData(
                table: "Educators",
                columns: new[] { "Id", "FirstName", "LastName", "MiddleName" },
                values: new object[,]
                {
                    { new Guid("0322ca1f-2d3c-4759-8cbb-17f839785cd8"), "Prepod", "Prepodov", "Prepodovich" },
                    { new Guid("64f29e16-283a-49af-a52a-906a2b16b515"), "Educator", "Educatorov", "Educatorovich" }
                });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 1,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 28, 3, 20, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 28, 1, 45, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 2,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 28, 5, 10, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 28, 3, 35, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 3,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 28, 7, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 28, 5, 25, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 4,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 28, 9, 20, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 28, 7, 45, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 5,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 28, 11, 10, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 28, 9, 35, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 6,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 28, 13, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 28, 11, 25, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 7,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 28, 14, 50, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 28, 13, 15, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccountType", "ClaimedRoles", "ClusterNumber", "Email", "FirstName", "GradeBookNumber", "LastName", "MiddleName", "Password", "RefreshToken", "RefreshTokenExpiryTime", "ScheduleSelfId", "VerifiedRoles" },
                values: new object[] { new Guid("83e40664-59b7-4e3a-8229-9f224ef2eb6a"), 0, new int[0], null, "root@root.net", null, null, null, null, "aboba", null, null, null, new[] { 0 } });

            migrationBuilder.CreateIndex(
                name: "IX_CampusBuildings_Address",
                table: "CampusBuildings",
                column: "Address",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CampusBuildings_Address",
                table: "CampusBuildings");

            migrationBuilder.DeleteData(
                table: "Educators",
                keyColumn: "Id",
                keyValue: new Guid("0322ca1f-2d3c-4759-8cbb-17f839785cd8"));

            migrationBuilder.DeleteData(
                table: "Educators",
                keyColumn: "Id",
                keyValue: new Guid("64f29e16-283a-49af-a52a-906a2b16b515"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("83e40664-59b7-4e3a-8229-9f224ef2eb6a"));

            migrationBuilder.InsertData(
                table: "Clusters",
                columns: new[] { "Number", "SuperClusterNumber" },
                values: new object[] { "9721", null });

            migrationBuilder.InsertData(
                table: "Educators",
                columns: new[] { "Id", "FirstName", "LastName", "MiddleName" },
                values: new object[,]
                {
                    { new Guid("878b71be-0f64-4893-a75a-1dd0205609d5"), "Prepod", "Prepodov", "Prepodovich" },
                    { new Guid("f718d1e0-f6ce-4c16-ba28-00ec621f0cce"), "Educator", "Educatorov", "Educatorovich" }
                });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 1,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 24, 3, 20, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 24, 1, 45, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 2,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 24, 5, 10, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 24, 3, 35, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 3,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 24, 7, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 24, 5, 25, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 4,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 24, 9, 20, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 24, 7, 45, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 5,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 24, 11, 10, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 24, 9, 35, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 6,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 24, 13, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 24, 11, 25, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 7,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 24, 14, 50, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 24, 13, 15, 0, 0, DateTimeKind.Utc) });
        }
    }
}
