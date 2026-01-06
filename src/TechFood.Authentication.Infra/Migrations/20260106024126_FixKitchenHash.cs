using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechFood.Authentication.Infra.Migrations
{
    /// <inheritdoc />
    public partial class FixKitchenHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ServiceClients",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e5f6-a7b8-4c5d-0e1f-2a3b4c5d6e7f"),
                column: "ClientSecretHash",
                value: "AQAAAAIAAYagAAAAENxQDaPMNdL7YhDY9bM7SSs1lWJM0EJfdbqvLBUT0w/Lec7RhsmE4bWUbtojlDbJLA==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ServiceClients",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e5f6-a7b8-4c5d-0e1f-2a3b4c5d6e7f"),
                column: "ClientSecretHash",
                value: "AAQAAAAIAAYagAAAAEK0Ly9jwdR3uEE1dXSiXeN6Zqpnvz2XWdEKTcaoc+MBGSvoj31/sGh4wlH3WEggI5Q==");
        }
    }
}
