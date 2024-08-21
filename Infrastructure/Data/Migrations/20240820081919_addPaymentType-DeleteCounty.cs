using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class addPaymentTypeDeleteCounty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "County",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "PaymentType",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "0 = LinePay, 1 = ECPay");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "County",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
