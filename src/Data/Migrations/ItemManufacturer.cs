using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Data.Migrations;

public partial class ItemManufacturer : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

        migrationBuilder.AddColumn<int>("ManufacturerId",
            "Items",
            "integer",
            nullable: true);

        migrationBuilder.CreateTable("Manufacturers",
            table => new
            {
                Id = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>("text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Manufacturers", x => x.Id);
            });

        migrationBuilder.CreateIndex("IX_Items_ManufacturerId",
            "Items",
            "ManufacturerId");

        migrationBuilder.AddForeignKey("FK_Items_Manufacturers_ManufacturerId",
            "Items",
            "ManufacturerId",
            "Manufacturers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey("FK_Items_Manufacturers_ManufacturerId",
            "Items");

        migrationBuilder.DropTable("Manufacturers");

        migrationBuilder.DropIndex("IX_Items_ManufacturerId",
            "Items");

        migrationBuilder.DropColumn("ManufacturerId",
            "Items");

        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");
    }
}