using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutOfTimePrototype.Migrations
{
    /// <inheritdoc />
    public partial class ClassManyClusters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Clusters_ClusterNumber",
                table: "Classes");

            migrationBuilder.DropIndex(
                name: "IX_Classes_ClusterNumber",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "ClusterNumber",
                table: "Classes");

            migrationBuilder.CreateTable(
                name: "ClassCluster",
                columns: table => new
                {
                    ClassesId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClustersNumber = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassCluster", x => new { x.ClassesId, x.ClustersNumber });
                    table.ForeignKey(
                        name: "FK_ClassCluster_Classes_ClassesId",
                        column: x => x.ClassesId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassCluster_Clusters_ClustersNumber",
                        column: x => x.ClustersNumber,
                        principalTable: "Clusters",
                        principalColumn: "Number",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateIndex(
                name: "IX_ClassCluster_ClustersNumber",
                table: "ClassCluster",
                column: "ClustersNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassCluster");

            migrationBuilder.AddColumn<string>(
                name: "ClusterNumber",
                table: "Classes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 1,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 14, 3, 20, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 14, 1, 45, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 2,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 14, 5, 10, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 14, 3, 35, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 3,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 14, 7, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 14, 5, 25, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 4,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 14, 9, 20, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 14, 7, 45, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 5,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 14, 11, 10, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 14, 9, 35, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 6,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 14, 13, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 14, 11, 25, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Number",
                keyValue: 7,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2023, 2, 14, 14, 50, 0, 0, DateTimeKind.Utc), new DateTime(2023, 2, 14, 13, 15, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.CreateIndex(
                name: "IX_Classes_ClusterNumber",
                table: "Classes",
                column: "ClusterNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Clusters_ClusterNumber",
                table: "Classes",
                column: "ClusterNumber",
                principalTable: "Clusters",
                principalColumn: "Number",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
