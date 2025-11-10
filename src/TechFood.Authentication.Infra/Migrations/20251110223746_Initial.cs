using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TechFood.Authentication.Infra.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceClients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    ClientSecretHash = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Scopes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceClients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameFullName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Username = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    EmailAddress = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    PasswordHash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "varchar(50)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ServiceClients",
                columns: new[] { "Id", "ClientId", "ClientSecretHash", "CreatedAt", "IsActive", "IsDeleted", "LastUsedAt", "Name", "Scopes" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d"), "order-service", "AQAAAAIAAYagAAAAEGc5qkWBdWL8/9BHGSyC24vz49eu41YPzSg0AaUFWNzA/qJgKZQy0dK+BiUDwo45qw==", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, "Order Service", "orders.read,orders.write,users.read" },
                    { new Guid("b2c3d4e5-f6a7-4b5c-9d0e-1f2a3b4c5d6e"), "payment-service", "AQAAAAIAAYagAAAAEJ1h6kwzl70PTafDv/XTtOK0OD5fgj+7ushepfeOcmT2fagJajeYNh0jeQSo7YYIlQ==", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, "Payment Service", "payments.read,payments.write,orders.read,users.read" },
                    { new Guid("c3d4e5f6-a7b8-4c5d-0e1f-2a3b4c5d6e7f"), "kitchen-service", "AAQAAAAIAAYagAAAAEK0Ly9jwdR3uEE1dXSiXeN6Zqpnvz2XWdEKTcaoc+MBGSvoj31/sGh4wlH3WEggI5Q==", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, "Kitchen Service", "kitchen.read,kitchen.write,orders.read,users.read" },
                    { new Guid("d4e5f6a7-b8c9-4d5e-1f2a-3b4c5d6e7f8a"), "backoffice-service", "AQAAAAIAAYagAAAAEJlLak1GYPXjel9Lc9LhwpLilkyUvFhwBuxNL0g8sn6POZv3fD+XqHN0C90pJbE9vg==", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, "Backoffice", "orders.read,orders.write,payments.read,payments.write,users.read,users.write,kitchen.read,kitchen.write" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "IsDeleted", "PasswordHash", "Role", "Username", "EmailAddress", "NameFullName" },
                values: new object[] { new Guid("fa09f3a0-f22d-40a8-9cca-0c64e5ed50e4"), false, "AQAAAAIAAYagAAAAEKs0I0Zk5QKKieJTm20PwvTmpkSfnp5BhSl5E35ny8DqffCJA+CiDRnnKRCeOx8+mg==", "admin", "john.admin", "john.admin@techfood.com", "John Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceClients_ClientId",
                table: "ServiceClients",
                column: "ClientId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceClients");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
