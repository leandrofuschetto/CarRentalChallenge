using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRental.Data.Migrations
{
    public partial class InsertRentals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
               table: "Rentals",
               columns: new[] { "RentalId", "VehicleId", "ClientId", "DateFrom", "DateTo", "Price", "Active" },
               values: new object[,]
               {
                    { 1, 1, 1, "2022-09-01", "2022-09-05", 100, true },
                    { 2, 2, 2, "2022-09-01", "2022-09-11", 150, true },
                    { 3, 5, 1, "2022-09-07", "2022-09-15", 120, true }
               });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Rentals",
                keyColumn: "RentalId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Rentals",
                keyColumn: "RentalId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Rentals",
                keyColumn: "RentalId",
                keyValue: 3);
        }
    }
}
