using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRental.Data.Migrations
{
    public partial class InsertClients : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "ClientId", "Fullname", "Email", "Active" },
                values: new object[,]
                {
                    { 1, "Lionel Messi", "leiton10@gmail.com", true },
                    { 2, "Diego Maradona", "diegote@gmail.com", true },
                    { 3, "Gabriel Batistuta", "batigol@gmail.com", false },
                    { 4, "Diego Simeone", "elcholo_8@gmail.com", true },
                    { 5, "Angel Di Maria", "angelito@gmail.com", true }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "ClientId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ClientId",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ClientId",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ClientId",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ClientId",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
