using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRental.Data.Migrations
{
    public partial class InsertUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
               table: "Users",
               columns: new[] { "Id", "Username", "Password", "Salt" },
               values: new object[,]
               {
                    { 1, "JuanPerez", "LNw3OUTQgXqzQxGlt5IeeMzdayWiLLkAbOCI0BAm9aY=", "S1joR6sEUjja+FymlBj+Lw==" } 
                   //pass Juan1234
               });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
