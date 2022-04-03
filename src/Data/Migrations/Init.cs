using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Data.Migrations;

public partial class Init : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

        migrationBuilder.CreateTable("Categories",
            table => new
            {
                Id = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>("text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Categories", x => x.Id);
            });

        migrationBuilder.CreateTable("Places",
            table => new
            {
                Id = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>("text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Places", x => x.Id);
            });

        migrationBuilder.CreateTable("Users",
            table => new
            {
                Id = table.Column<long>("bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                FirstName = table.Column<string>("text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.CreateTable("Items",
            table => new
            {
                Id = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>("text", nullable: true),
                CategoryId = table.Column<int>("integer", nullable: true),
                PlaceId = table.Column<int>("integer", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Items", x => x.Id);

                table.ForeignKey("FK_Items_Categories_CategoryId",
                    x => x.CategoryId,
                    "Categories",
                    "Id",
                    onDelete: ReferentialAction.Restrict);

                table.ForeignKey("FK_Items_Places_PlaceId",
                    x => x.PlaceId,
                    "Places",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable("Ratings",
            table => new
            {
                Id = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Value = table.Column<int>("integer", nullable: false),
                ItemId = table.Column<int>("integer", nullable: false),
                UserId = table.Column<long>("bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Ratings", x => x.Id);

                table.ForeignKey("FK_Ratings_Items_ItemId",
                    x => x.ItemId,
                    "Items",
                    "Id",
                    onDelete: ReferentialAction.Cascade);

                table.ForeignKey("FK_Ratings_Users_UserId",
                    x => x.UserId,
                    "Users",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex("IX_Items_CategoryId",
            "Items",
            "CategoryId");

        migrationBuilder.CreateIndex("IX_Items_PlaceId",
            "Items",
            "PlaceId");

        migrationBuilder.CreateIndex("IX_Ratings_ItemId",
            "Ratings",
            "ItemId");

        migrationBuilder.CreateIndex("IX_Ratings_UserId",
            "Ratings",
            "UserId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("Ratings");

        migrationBuilder.DropTable("Items");

        migrationBuilder.DropTable("Users");

        migrationBuilder.DropTable("Categories");

        migrationBuilder.DropTable("Places");
    }
}