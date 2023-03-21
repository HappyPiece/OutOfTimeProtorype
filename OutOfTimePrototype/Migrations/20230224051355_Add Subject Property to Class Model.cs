using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OutOfTimePrototype.Migrations
{
    /// <inheritdoc />
    public partial class AddSubjectPropertytoClassModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Educators",
                keyColumn: "Id",
                keyValue: new Guid("74253563-1472-4fd8-9cc5-21e6120c8a45"));

            migrationBuilder.DeleteData(
                table: "Educators",
                keyColumn: "Id",
                keyValue: new Guid("b567b9e6-4c7d-4a28-970c-e2462512de57"));

            migrationBuilder.AddColumn<Guid>(
                name: "SubjectId",
                table: "Classes",
                type: "uuid",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Classes_SubjectId",
                table: "Classes",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Subjects_SubjectId",
                table: "Classes",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Subjects_SubjectId",
                table: "Classes");

            migrationBuilder.DropIndex(
                name: "IX_Classes_SubjectId",
                table: "Classes");

            migrationBuilder.DeleteData(
                table: "Educators",
                keyColumn: "Id",
                keyValue: new Guid("878b71be-0f64-4893-a75a-1dd0205609d5"));

            migrationBuilder.DeleteData(
                table: "Educators",
                keyColumn: "Id",
                keyValue: new Guid("f718d1e0-f6ce-4c16-ba28-00ec621f0cce"));

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Classes");

            migrationBuilder.InsertData(
                table: "Educators",
                columns: new[] { "Id", "FirstName", "LastName", "MiddleName" },
                values: new object[,]
                {
                    { new Guid("74253563-1472-4fd8-9cc5-21e6120c8a45"), "Prepod", "Prepodov", "Prepodovich" },
                    { new Guid("b567b9e6-4c7d-4a28-970c-e2462512de57"), "Educator", "Educatorov", "Educatorovich" }
                });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 1,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 22, 3, 20, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 22, 1, 45, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 2,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 22, 5, 10, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 22, 3, 35, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 3,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 22, 7, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 22, 5, 25, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 4,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 22, 9, 20, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 22, 7, 45, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 5,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 22, 11, 10, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 22, 9, 35, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 6,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 22, 13, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 22, 11, 25, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 7,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 22, 14, 50, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 22, 13, 15, 0, 0, DateTimeKind.Utc) });
        }
    }
}
