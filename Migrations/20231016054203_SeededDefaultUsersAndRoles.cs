using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace bookstoreApp.Api.Migrations
{
    /// <inheritdoc />
    /// // Questa classe rappresenta una migrazione e eredita dalla classe Migration.
    public partial class SeededDefaultUsersAndRoles : Migration
    {
        /// <inheritdoc />
        /// Questo metodo viene eseguito durante la migrazione "Up" per applicare le modifiche al database.
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4043d151-7160-4208-be8d-260607a5f812", null, "Administrator", "ADMINISTRATOR" },
                    { "cfdf272a-5bec-405f-a6eb-2ee80dc0ae4b", null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "072b9fc5-1b92-4441-b3a6-27cadc442cb9", 0, "8694e042-9cbf-4449-8e3f-08d620ad2ccb", "admin@bookstre.com", false, "System", "Admin", false, null, "ADMIN@BOOKSTORE.COM", "ADMIN@BOOKSTORE.COM", "AQAAAAIAAYagAAAAEBnimm3v073I0AJDJDkTgAZsOTBerefW3ygz4Q54ehRqn1PIXBtSmPQSd/8sUk1YVw==", null, false, "b6290e17-4715-4fb8-9dfa-653079cc4c7e", false, "admin@bookstre.com" },
                    { "5e75c0d7-5127-406e-aa70-b28f0cd57d83", 0, "973bf894-4e4a-4b7a-b81b-682b6e835c4b", "user@bookstre.com", false, "System", "User", false, null, "USER@BOOKSTORE.COM", "USER@BOOKSTORE.COM", "AQAAAAIAAYagAAAAEOCcSyP64Wn2mrfleg3D2f4X3ydLDv3QVkpS3vj+mH8Gu0a6Ku0Jkr7PyI/fmtXYEQ==", null, false, "f9224088-f8eb-4ce1-98a6-af905b0404a0", false, "user@bookstre.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "4043d151-7160-4208-be8d-260607a5f812", "072b9fc5-1b92-4441-b3a6-27cadc442cb9" },
                    { "cfdf272a-5bec-405f-a6eb-2ee80dc0ae4b", "5e75c0d7-5127-406e-aa70-b28f0cd57d83" }
                });
        }

        /// <inheritdoc />
        /// Questo metodo viene eseguito durante la migrazione "Down" per annullare le modifiche al database.
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "4043d151-7160-4208-be8d-260607a5f812", "072b9fc5-1b92-4441-b3a6-27cadc442cb9" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "cfdf272a-5bec-405f-a6eb-2ee80dc0ae4b", "5e75c0d7-5127-406e-aa70-b28f0cd57d83" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4043d151-7160-4208-be8d-260607a5f812");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cfdf272a-5bec-405f-a6eb-2ee80dc0ae4b");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "072b9fc5-1b92-4441-b3a6-27cadc442cb9");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5e75c0d7-5127-406e-aa70-b28f0cd57d83");
        }
    }
}
