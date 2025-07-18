using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Freelancer_Task.Migrations
{
    /// <inheritdoc />
    public partial class AddSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "TimeSpent",
                table: "Tasks",
                newName: "TotalTimeSpent");

            migrationBuilder.AddColumn<DateTime>(
                name: "TimerStart",
                table: "Tasks",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimerStart",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "TotalTimeSpent",
                table: "Tasks",
                newName: "TimeSpent");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Tasks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
