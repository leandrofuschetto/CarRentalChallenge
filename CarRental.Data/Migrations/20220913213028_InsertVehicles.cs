using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRental.Data.Migrations
{
    public partial class InsertVehicles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
               table: "Vehicles",
               columns: new[] { "Id", "Model", "PricePerDay", "Active" },
               values: new object[,]
               {
                    { 1, "Astra", 12.2, true },
                    { 2, "Fiesta", 12, true },
                    { 3, "Corsa", 10.5, false },
                    { 4, "Focus", 13.1, true },
                    { 5, "Palio", 7.2,  true }
               });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Ids",
                keyValue: 5);
        }
    }
}
