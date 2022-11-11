using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace newTolkuchka.Models.Migs
{
    public partial class _001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: false),
                    NameRu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameTm = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsForHome = table.Column<bool>(type: "bit", nullable: false),
                    NotInUse = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PriceRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RealRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Entries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Employee = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Act = table.Column<int>(type: "int", nullable: false),
                    Entity = table.Column<int>(type: "int", nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Slides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Layout = table.Column<int>(type: "int", nullable: false),
                    NotInUse = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slides", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Specs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameRu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameTm = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsFilter = table.Column<bool>(type: "bit", nullable: false),
                    IsImaged = table.Column<bool>(type: "bit", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    NamingOrder = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    PhoneMain = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PhoneSecondary = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameRu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameTm = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsPhoneConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    Pin = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BirthDay = table.Column<DateTime>(type: "Date", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Warranties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameRu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameTm = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warranties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BrandId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lines_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoryAdLinks",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    StepParentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryAdLinks", x => new { x.CategoryId, x.StepParentId });
                    table.ForeignKey(
                        name: "FK_CategoryAdLinks_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Hash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PositionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecsValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameRu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameTm = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SpecId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecsValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpecsValues_Specs_SpecId",
                        column: x => x.SpecId,
                        principalTable: "Specs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseInvoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CurrencyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoices_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoices_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Buyer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    InvoiceAddress = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    InvoiceEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InvoicePhone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DeliveryCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrencyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    IsDelivered = table.Column<bool>(type: "bit", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invoices_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Models",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BrandId = table.Column<int>(type: "int", nullable: false),
                    LineId = table.Column<int>(type: "int", nullable: true),
                    DescRu = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: false),
                    DescEn = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: false),
                    DescTm = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Models", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Models_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Models_Lines_LineId",
                        column: x => x.LineId,
                        principalTable: "Lines",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SpecsValueMods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameRu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameTm = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SpecsValueId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecsValueMods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpecsValueMods_SpecsValues_SpecsValueId",
                        column: x => x.SpecsValueId,
                        principalTable: "SpecsValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModelSpecs",
                columns: table => new
                {
                    ModelId = table.Column<int>(type: "int", nullable: false),
                    SpecId = table.Column<int>(type: "int", nullable: false),
                    IsNameUse = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelSpecs", x => new { x.ModelId, x.SpecId });
                    table.ForeignKey(
                        name: "FK_ModelSpecs_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModelSpecs_Specs_SpecId",
                        column: x => x.SpecId,
                        principalTable: "Specs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NewPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NotInUse = table.Column<bool>(type: "bit", nullable: false),
                    IsRecommended = table.Column<bool>(type: "bit", nullable: false),
                    IsNew = table.Column<bool>(type: "bit", nullable: false),
                    OnOrder = table.Column<bool>(type: "bit", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    TypeId = table.Column<int>(type: "int", nullable: false),
                    BrandId = table.Column<int>(type: "int", nullable: false),
                    LineId = table.Column<int>(type: "int", nullable: true),
                    ModelId = table.Column<int>(type: "int", nullable: true),
                    WarrantyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_Lines_LineId",
                        column: x => x.LineId,
                        principalTable: "Lines",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Products_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Products_Types_TypeId",
                        column: x => x.TypeId,
                        principalTable: "Types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_Warranties_WarrantyId",
                        column: x => x.WarrantyId,
                        principalTable: "Warranties",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CategoryProductAdLinks",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryProductAdLinks", x => new { x.CategoryId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_CategoryProductAdLinks_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryProductAdLinks_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductSpecsValueMods",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    SpecsValueModId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSpecsValueMods", x => new { x.ProductId, x.SpecsValueModId });
                    table.ForeignKey(
                        name: "FK_ProductSpecsValueMods_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductSpecsValueMods_SpecsValueMods_SpecsValueModId",
                        column: x => x.SpecsValueModId,
                        principalTable: "SpecsValueMods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductSpecsValues",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    SpecsValueId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSpecsValues", x => new { x.ProductId, x.SpecsValueId });
                    table.ForeignKey(
                        name: "FK_ProductSpecsValues_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductSpecsValues_SpecsValues_SpecsValueId",
                        column: x => x.SpecsValueId,
                        principalTable: "SpecsValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Purchases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PurchaseInvoiceId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Purchases_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Purchases_PurchaseInvoices_PurchaseInvoiceId",
                        column: x => x.PurchaseInvoiceId,
                        principalTable: "PurchaseInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wishes",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wishes", x => new { x.UserId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_Wishes_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Wishes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InvoiceId = table.Column<int>(type: "int", nullable: false),
                    PurchaseId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Purchases_PurchaseId",
                        column: x => x.PurchaseId,
                        principalTable: "Purchases",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Apple" },
                    { 2, "Samsung" },
                    { 3, "DELL" },
                    { 4, "Adidas" },
                    { 5, "Reebok" },
                    { 6, "Kensington" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "IsForHome", "NameEn", "NameRu", "NameTm", "NotInUse", "Order", "ParentId" },
                values: new object[,]
                {
                    { 1, true, "Office equipment", "Оргтехника", "Ofis tehnikasy", false, 0, 0 },
                    { 2, false, "Mobile devices", "Мобильные устройства", "Mobil gurluşlar", false, 1, 0 },
                    { 3, true, "Home equipment", "Бытовая техника", "Öy tehnikasy", false, 2, 0 },
                    { 4, true, "Clothing and textiles", "Одежда и текстиль", "Eşik we tekstil", false, 3, 0 },
                    { 5, true, "Food", "Еда", "Iýmit", false, 4, 0 },
                    { 6, false, "Computers", "Компьютеры", "Kompýuterler", false, 0, 1 },
                    { 7, false, "Computer parts", "Компьютерные комплектующие", "Kompýuter enjamlary", false, 1, 1 },
                    { 8, false, "Monobloks", "Моноблоки", "Monobloklar", false, 0, 6 },
                    { 9, false, "Notebooks", "Ноутбуки", "Noutbuklar", false, 1, 6 },
                    { 10, false, "Phones, tabs & accessories", "Телефоны, планшеты и аксессуары", "Telefonlar, planşetler we aksesuarlar", false, 0, 2 },
                    { 11, false, "Mobile phones", "Мобильные телефоны", "Öyjükli telefonlar", false, 0, 10 },
                    { 12, false, "Tablets", "Планшеты", "Planşetler", false, 1, 10 },
                    { 13, false, "Brand PCs", "Брендовые компьютеры", "Brend kompýuterler", false, 2, 6 },
                    { 15, false, "Notebook stands", "Подставки для ноутбуков", "Noutbuk goltuklary", false, 0, 17 },
                    { 17, false, "PC accessories", "Аксессуары для компьютеров", "Kompýuter esbaplary", false, 2, 1 }
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "CodeName", "PriceRate", "RealRate" },
                values: new object[,]
                {
                    { 1, "USD", 1m, 1m },
                    { 2, "TMT", 20.5m, 19.5m }
                });

            migrationBuilder.InsertData(
                table: "Positions",
                columns: new[] { "Id", "Level", "Name" },
                values: new object[,]
                {
                    { 1, 3, "Владелец" },
                    { 2, 3, "Программист" },
                    { 3, 2, "Менеджер" }
                });

            migrationBuilder.InsertData(
                table: "Slides",
                columns: new[] { "Id", "Layout", "Link", "Name", "NotInUse" },
                values: new object[,]
                {
                    { 1, 0, "#", "Скидки на сайте tolkuchka.bar", false },
                    { 2, 0, "#", "Акции на сайте tolkuchka.bar", false }
                });

            migrationBuilder.InsertData(
                table: "Specs",
                columns: new[] { "Id", "IsFilter", "IsImaged", "NameEn", "NameRu", "NameTm", "NamingOrder", "Order" },
                values: new object[,]
                {
                    { 1, true, true, "Color", "Цвет", "Reňk", 2, 1 },
                    { 2, true, false, "Memory", "Память", "Ýat", null, 2 },
                    { 3, true, false, "Storage", "Накопитель", "Ýatda saklaýjy", 1, 3 },
                    { 4, false, false, "Weight", "Вес", "Agram", null, 4 },
                    { 5, false, false, "Material", "Материал", "Material", null, 0 }
                });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "Address", "Name", "PhoneMain", "PhoneSecondary" },
                values: new object[,]
                {
                    { 1, "11 мкр-н, ул. А.Ниязова 44", "Ak yol", "280401", "726849" },
                    { 2, "Оптовка", "Hajy", "99364919890", null }
                });

            migrationBuilder.InsertData(
                table: "Types",
                columns: new[] { "Id", "NameEn", "NameRu", "NameTm" },
                values: new object[,]
                {
                    { 1, "Mobile phone", "Мобильный телефон", "Öýjukli telefon" },
                    { 2, "Laptop", "Ноутбук", "Noutbuk" },
                    { 3, "Monoblock", "Моноблок", "Monoblok" },
                    { 4, "Refrigerator", "Холодильник", "Doňduryjy" },
                    { 5, "Notebook stand", "Подставка для ноутбука", "Noutbuk goltugy" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "BirthDay", "Email", "IsPhoneConfirmed", "Name", "Phone", "Pin" },
                values: new object[] { 1, "Magtymguly 150/10", new DateTime(1984, 5, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "durdysagty@mail.ru", false, "Durdy", "99365811482", "8/Bljhsa9Fyja/87ddqryA==" });

            migrationBuilder.InsertData(
                table: "Warranties",
                columns: new[] { "Id", "NameEn", "NameRu", "NameTm" },
                values: new object[] { 1, "1 week", "1 неделя", "1 hepde" });

            migrationBuilder.InsertData(
                table: "Warranties",
                columns: new[] { "Id", "NameEn", "NameRu", "NameTm" },
                values: new object[,]
                {
                    { 2, "1 month", "1 месяц", "1 aý" },
                    { 3, "3 months", "3 месяца", "3 aý" },
                    { 4, "6 months", "6 месяцев", "6 aý" },
                    { 5, "1 year", "1 год", "1 ýyl" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Hash", "Login", "Password", "PositionId" },
                values: new object[,]
                {
                    { 1, "1", "ayna", "3XX32n4h8NrkNSYU3gvK2g==", 1 },
                    { 2, "1", "durdy", "toc3DuDZ9Sppru9CHY/M5g==", 2 }
                });

            migrationBuilder.InsertData(
                table: "Lines",
                columns: new[] { "Id", "BrandId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "iPhone" },
                    { 2, 1, "iPad" },
                    { 3, 1, "Macbook" },
                    { 4, 2, "Galaxy" },
                    { 5, 2, "The Frame" },
                    { 6, 3, "Latitude" },
                    { 7, 3, "Inspiron" },
                    { 8, 4, "Yeezy" },
                    { 9, 4, "Predator" }
                });

            migrationBuilder.InsertData(
                table: "Models",
                columns: new[] { "Id", "BrandId", "DescEn", "DescRu", "DescTm", "LineId", "Name" },
                values: new object[] { 5, 6, "The patented SmartFit system allows you to adjust the height and angle of your laptop screen using the included hand chart to find your personal comfort color while reducing neck strain and eye strain.The Easy Riser lifts your laptop off its desktop, promoting airflow to improve battery performance and ease stress on internal components.", "Запатентованная система SmartFit позволяет настраивать высоту и угол экрана вашего ноутбука с помощью прилагаемой ручной диаграммы, чтобы найти свой личный цвет комфорта, уменьшая напряжение шеи и напряжение глаз. Easy Riser снимает ваш ноутбук со своего рабочего стола, способствуя воздушному потоку, чтобы улучшить работу аккумулятора и облегчить нагрузку на внутренние компоненты.", "Patentlenen SmartFit ulgamy, boýnuň süzülmesini we gözüň dartylmagyny azaltmak bilen şahsy rahatlyk reňkini tapmak üçin goşulan el diagrammasyny ulanyp, noutbuk ekranyňyzyň beýikligini we burçuny sazlamaga mümkinçilik berýär. “Easy Riser”, batareýanyň işleýşini gowulandyrmak we içerki böleklere edilýän stresleri ýeňilleşdirmek üçin noutbukyňyzy iş stolundan çykarýar.", null, "B2152B" });

            migrationBuilder.InsertData(
                table: "SpecsValues",
                columns: new[] { "Id", "NameEn", "NameRu", "NameTm", "SpecId" },
                values: new object[,]
                {
                    { 1, "Black", "Черный", "Gara", 1 },
                    { 2, "White", "Белый", "Ak", 1 },
                    { 3, "Blue", "Синий", "Gök", 1 },
                    { 4, "Red", "Красный", "Gyzyl", 1 },
                    { 5, "Green", "Зеленый", "Ýaşyl", 1 },
                    { 6, "Gray", "Серый", "Çal", 1 },
                    { 7, "Yellow", "Желтый", "Sary", 1 },
                    { 8, "Gold", "Золотой", "Altynsow", 1 },
                    { 9, "Silver", "Серебристый", "Kümüşsow", 1 },
                    { 10, "Brown", "Коричневый", "Goňur", 1 },
                    { 11, "2GB", "2ГБ", "2GB", 2 },
                    { 12, "4GB", "4ГБ", "4GB", 2 },
                    { 13, "6GB", "6ГБ", "6GB", 2 },
                    { 14, "8GB", "8ГБ", "8GB", 2 },
                    { 15, "12GB", "12ГБ", "12GB", 2 },
                    { 16, "16GB", "16ГБ", "16GB", 2 },
                    { 17, "32GB", "32ГБ", "32GB", 2 },
                    { 18, "64GB", "64ГБ", "64GB", 2 },
                    { 19, "128GB", "128ГБ", "128GB", 2 },
                    { 20, "256GB", "256ГБ", "256GB", 2 },
                    { 21, "8GB", "8ГБ", "8GB", 3 },
                    { 22, "16GB", "16ГБ", "16GB", 3 },
                    { 23, "32GB", "32ГБ", "32GB", 3 },
                    { 24, "64GB", "64ГБ", "64GB", 3 },
                    { 25, "128GB", "128ГБ", "128GB", 3 },
                    { 26, "256GB", "256ГБ", "256GB", 3 },
                    { 27, "512GB", "512ГБ", "512GB", 3 },
                    { 28, "1TB", "1ТБ", "1TB", 3 },
                    { 29, "2TB", "2ТБ", "2TB", 3 },
                    { 30, "3TB", "3ТБ", "3TB", 3 }
                });

            migrationBuilder.InsertData(
                table: "SpecsValues",
                columns: new[] { "Id", "NameEn", "NameRu", "NameTm", "SpecId" },
                values: new object[,]
                {
                    { 31, "4TB", "4ТБ", "4TB", 3 },
                    { 32, "5TB", "5ТБ", "5TB", 3 },
                    { 33, "6TB", "6ТБ", "6TB", 3 },
                    { 34, "8TB", "8ТБ", "8TB", 3 },
                    { 35, "10TB", "10ТБ", "10TB", 3 },
                    { 36, "12TB", "12ТБ", "12TB", 3 },
                    { 37, "144g", "144гр", "144g", 4 },
                    { 38, "212g", "212гр", "212g", 4 },
                    { 39, "570g", "570гр", "570g", 4 },
                    { 40, "1.1kg", "1.1кг", "1.1kg", 4 },
                    { 41, "174g", "174гр", "174g", 4 },
                    { 42, "Pink", "Розовый", "Gülgüne", 1 },
                    { 43, "203g", "203гр", "203g", 4 },
                    { 44, "238g", "238гр", "238g", 4 },
                    { 45, "Plastic", "Пластик", "Plastik", 5 },
                    { 46, "Metal", "Металл", "Metal", 5 },
                    { 47, "Glass", "Стекло", "Aýna", 5 },
                    { 48, "Metal and glass", "Металл и стекло", "Metal we aýna", 5 },
                    { 49, "Metal and plastic", "Металл и пластик", "Metal we plastik", 5 },
                    { 50, "850g", "850гр", "850g", 4 },
                    { 51, "950g", "950гр", "950g", 4 },
                    { 52, "263g", "263гр", "263g", 4 },
                    { 53, "Beige", "Бежевый", "Bej", 1 }
                });

            migrationBuilder.InsertData(
                table: "ModelSpecs",
                columns: new[] { "ModelId", "SpecId", "IsNameUse" },
                values: new object[,]
                {
                    { 5, 1, true },
                    { 5, 4, false },
                    { 5, 5, false }
                });

            migrationBuilder.InsertData(
                table: "Models",
                columns: new[] { "Id", "BrandId", "DescEn", "DescRu", "DescTm", "LineId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Powerful processor, elegant design, extended screen color range, the ability to connect two pairs of headphones at the same time, and much more - you get it all with the incredible performance of the 32nd generation Apple iPhone SE! It has everything you love - and more! Apple iPhone SE 3 is equipped with the fastest A15 Bionic processor, which performs any tasks quickly and efficiently! This is a real technology of the future, which combines low energy consumption with high performance. Apple iPhone SE 3 is designed to be your ideal smartphone!", "Мощный процессор, элегантный дизайн, расширенный цветовой диапазон экрана, возможность подключения двух пар наушников одновременно, и многое другое — все это вы получите с невероятно производительным Apple iPhone SE 32-го поколения! В нём есть все что вы любите — и даже больше! Apple iPhone SE 3 оснащен самым быстрым процессором A15 Bionic, который выполняет любые поставленные задачи быстро и эффективно! Это настоящая технология будущего, которая совмещает в себе низкую энергозатратность при высокой работоспособности. Apple iPhone SE 3 создан, чтобы стать вашим идеальным смартфоном!", "Güýçli prosessor, ajaýyp dizaýn, giňeldilen ekran reňk diapazony, bir wagtyň özünde iki jübüt nauşnik birikdirmek ukyby we başga-da köp zat - hemmesini 32-nji nesil Apple iPhone SE-niň ajaýyp öndürijiligi bilen alarsyňyz! Onda siziň söýýän zatlaryňyzyň hemmesi we başgalar bar! Apple iPhone SE 3, islendik meseläni çalt we netijeli ýerine ýetirýän iň çalt A15 Bionic prosessor bilen enjamlaşdyrylandyr! Bu, pes energiýa sarp edilişini ýokary öndürijilik bilen birleşdirýän geljegiň hakyky tehnologiýasy. Apple iPhone SE 3 ideal smartfon siziň üçin döredildi!", 1, "SE 2022" },
                    { 2, 1, "Super Retina XDR display with ProMotion technology and fast, smooth response. A massive upgrade to the camera system that opens up completely new possibilities. Exceptional strength. A15 Bionic is the fastest iPhone chip. And impressive battery life. Everything is Pro. Surgical stainless steel, Ceramic Shield panel, reliable water protection (IP68) - all this is incredibly beautiful and exceptionally durable. Meet the Super Retina XDR display with ProMotion technology. It has an adaptive refresh rate up to 120Hz and amazing graphics performance - touch and be amazed.", "Дисплей Super Retina XDR с технологией ProMotion и быстрым, плавным откликом. Грандиозный апгрейд системы камер, открывающий совершенно новые возможности. Исключительная прочность. A15 Bionic — самый быстрый чип для iPhone. И впечатляющее время работы без подзарядки. Всё это Pro. Хирургическая нержавеющая сталь, панель Ceramic Shield, надёжная защита от воды (IP68) — всё это невероятно красиво и исключительно прочно. Встречайте дисплей Super Retina XDR с технологией ProMotion. У него адаптивная частота обновления до 120 Гц и великолепная графическая производительность — прикоснитесь и удивитесь.", "ProMotion tehnologiýasy we çalt, rahat seslenme bilen Super Retina XDR displeýi. Doly täze mümkinçilikleri açýan kamera ulgamyna köpçülikleýin täzeleniş. Adatdan daşary güýç. “A15 Bionic” iň çalt “iPhone” çipidir. Batareýanyň täsirli ömri. Hemme zat Pro. Hirurgiki poslamaýan polat, keramiki galkan paneli, ygtybarly suw goragy (IP68) - bularyň hemmesi ajaýyp owadan we ajaýyp. ProMotion tehnologiýasy bilen Super Retina XDR ekrany bilen tanyşyň. 120Hz-a çenli uýgunlaşdyrylan täzeleniş tizligi we ajaýyp grafiki öndürijiligi bar - degiň we haýran galyň.", 1, "13 Pro" },
                    { 3, 1, "Super Retina XDR display with ProMotion technology and fast, smooth response. A massive upgrade to the camera system that opens up completely new possibilities. Exceptional strength. A15 Bionic is the fastest iPhone chip. And impressive battery life. Everything is Pro. Surgical stainless steel, Ceramic Shield panel, reliable water protection (IP68) - all this is incredibly beautiful and exceptionally durable. This upgrade significantly upgrades both the hardware and the software. Macro mode is now available for the ultra-wide camera, 3x optical zoom for the telephoto camera, and night mode is supported on all three cameras.", "Дисплей Super Retina XDR с технологией ProMotion и быстрым, плавным откликом. Грандиозный апгрейд системы камер, открывающий совершенно новые возможности. Исключительная прочность. A15 Bionic — самый быстрый чип для iPhone. И впечатляющее время работы без подзарядки. Всё это Pro. Хирургическая нержавеющая сталь, панель Ceramic Shield, надёжная защита от воды (IP68) — всё это невероятно красиво и исключительно прочно. В этом апгрейде значительно обновлены и аппаратная часть, и программное обеспечение. Теперь для сверхширокоугольной камеры доступен режим макросъёмки, для телефотокамеры — трёхкратный оптический зум, а ночной режим поддерживается на всех трёх камерах.", "ProMotion tehnologiýasy we çalt, rahat seslenme bilen Super Retina XDR displeýi.Doly täze mümkinçilikleri açýan kamera ulgamyna köpçülikleýin täzeleniş.Adatdan daşary güýç. “A15 Bionic” iň çalt “iPhone” çipidir.Batareýanyň täsirli ömri.Hemme zat Pro.Hirurgiki poslamaýan polat, keramiki galkan paneli, ygtybarly suw goragy(IP68) - bularyň hemmesi ajaýyp owadan we ajaýyp.Bu täzelenme enjamlary we programma üpjünçiligini ep - esli ýokarlandyrýar.Ultra giň kamera üçin makro re modeimi, telefon kamerasy üçin 3x optiki ýakynlaşdyrma we üç kameranyň hemmesinde gijeki re modeim goldanýar.", 1, "13 Pro Max" },
                    { 4, 1, "The OLED display has become 28% brighter - up to 800 cd/ m². Everything is clearly visible on it even on the sunniest day. And the brightness when viewing content in HDR reaches 1200 cd/ m². You will be able to distinguish the smallest shades of black and white - as well as all other colors. At the same time, the display consumes battery power even more economically than before. The Super Retina XDR display features an incredibly high pixel density, making photos, videos and text look amazingly crisp. And thanks to the smaller area of ​​the TrueDepth camera, there is now more room for the image on the display.iPhone 13 has up to 2.5 hours longer battery life.The A15 Bionic processor and TrueDepth camera also power Face ID, an incredibly secure authentication technology.The ultra - fast A15 Bionic chip powers Cinema Effect, Photo Styles and more.Secure Enclave protects personal data, including Face ID and contacts.And the new chip increases the battery life.", "Дисплей OLED стал на 28% ярче — до 800 кд/ м². На нём всё хорошо видно даже в самый солнечный день. А яркость при просмотре контента в HDR достигает 1200 кд/ м². Вы сможете различить мельчайшие оттенки чёрного и белого — как и всех остальных цветов. При этом дисплей расходует заряд аккумулятора ещё более экономно, чем прежде. Дисплей Super Retina XDR отличается невероятно высокой плотностью пикселей — фотографии, видео и текст выглядят поразительно чётко. А благодаря уменьшенной площади камеры TrueDepth на дисплее теперь больше места для изображения. iPhone 13 работает от аккумулятора до 2,5 часов дольше. Процессор A15 Bionic и камера TrueDepth также обеспечивают работу Face ID, невероятно надёжной технологии аутентификации. Сверхбыстрый чип A15 Bionic обеспечивает работу режима «Киноэффект», фотографических стилей и других функций. Secure Enclave защищает персональные данные, в том числе Face ID и контакты. А ещё новый чип увеличивает время работы от аккумулятора.", "OLED ekrany 28 % ýagtylandy - 800 cd / m² çenli.Everythinghli zat iň güneşli günlerde - de aýdyň görünýär.HDR - de mazmuny göreniňde ýagtylygy 1200 cd / m² ýetýär.Gara we ak reňkleriň iň kiçi kölegelerini - beýleki reňkler ýaly tapawutlandyryp bilersiňiz.Şol bir wagtyň özünde, displeý batareýanyň güýjüni öňküsinden has tygşytly sarp edýär. “Super Retina XDR” displeýinde ajaýyp ýokary piksel dykyzlygy bar, suratlar, wideolar we tekst ajaýyp görünýär. “TrueDepth” kamerasynyň kiçi meýdany sebäpli indi ekranda şekil üçin has köp ýer bar. “iPhone 13” -iň batareýasynyň ömri 2, 5 sagada çenli.A15 Bionic prosessor we TrueDepth kamerasy, şeýle hem ygtybarly ygtybarly tanamak tehnologiýasy Face ID - i güýçlendirýär.Ultra çalt A15 Bionic çipi Kino effekti, Surat stilleri we ş.m.Howpsuz Enklaw, Face ID we aragatnaşyklary goşmak bilen şahsy maglumatlary goraýar.Täze çip bolsa batareýanyň ömrüni artdyrýar.", 1, "13" },
                    { 6, 2, "The Samsung Galaxy Fold is a smartphone that is changing the way we think about smartphones. The device combines a huge foldable screen with exceptionally powerful hardware.Multi - window mode displays 3 applications at once.The 7nm Snapdragon 855 processor is boosted by 12GB of RAM.The 512 GB flash drive is built with lightning - fast UFS 3.0 chips.Popular 3D games - PUBG, WoT: Blitz, Asphalt 9 and others - just “fly” here.Maximum pleasure.The unique mechanism will allow the screen to be folded.The reliability of the hinge has been tested 200,000 times.The device opens naturally,smoothly - like a book.The open position is clearly fixed. The connector module turns this smartphone into a compact gadget that is easy to carry around. Samsung Galaxy Fold is tomorrow's technology, available today.	Samsung Galaxy Fold, smartfonlar baradaky pikirimizi üýtgedýän smartfondyr.", "Samsung Galaxy Fold — смартфон, меняющий наше представление о смартфонах. Девайс сочетает огромный складной экран с исключительно мощным «железом». Многооконный режим отображает сразу 3 приложения. 7 - нанометровый процессор Snapdragon 855 усилен 12 ГБ оперативной памяти.Флэш - накопитель 512 ГБ построен на молниеносных чипах UFS 3.0.Популярные 3D - игры — PUBG, WoT: Blitz, Asphalt 9 и другие — здесь просто «летают». Удовольствие максимальное.Уникальный механизм позволят экрану складываться.Надежность шарнира проверена 200 000 раз.Девайс открывается естественно, плавно — как книга.Раскрытое положение четко фиксируется.Соединительный модуль превращает этот смартфон в компактный гаджет, который удобно носить с собой.Samsung Galaxy Fold — завтрашний день технологий, доступный уже сегодня.", "Enjam ullakan bukulýan ekrany gaty güýçli enjam bilen birleşdirýär. Köp penjire re modeimi birbada 3 programmany görkezýär. 7nm Snapdragon 855 prosessor 12 Gb RAM bilen güýçlendirilýär. 512 Gb fleş disk ýyldyrym çalt UFS 3.0 çipleri bilen gurlupdyr. Meşhur 3D oýunlary - PUBG, WoT: Blits, Asfalt 9 we başgalar - diňe şu ýerde “uçuň”. Iň ýokary lezzet. Üýtgeşik mehanizm ekrany bukmaga mümkinçilik berer. Çeňňegiň ygtybarlylygy 200,000 gezek synag edildi. Enjam kitap ýaly tebigy, rahat açylýar. Açyk pozisiýa anyk kesgitlenendir. Birikdiriji modul, bu smartfony daşamak aňsat bolan ykjam gadgeta öwürýär. Samsung Galaxy Fold, şu gün elýeterli ertirki tehnologiýa.", 4, "Z Fold 4" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BrandId", "CategoryId", "IsNew", "IsRecommended", "LineId", "ModelId", "NewPrice", "NotInUse", "OnOrder", "PartNo", "Price", "TypeId", "WarrantyId" },
                values: new object[] { 22, 6, 15, true, true, null, 5, 80.00m, false, false, null, 85.00m, 5, 2 });

            migrationBuilder.InsertData(
                table: "SpecsValueMods",
                columns: new[] { "Id", "NameEn", "NameRu", "NameTm", "SpecsValueId" },
                values: new object[,]
                {
                    { 1, "Midnight", "Полночь", "Ýary gije", 1 },
                    { 2, "Starlight", "Звездный свет", "Ýyldyz yşky", 2 },
                    { 3, "Graphite", "Графитовый", "Grafit", 6 },
                    { 4, "Graygreen", "Серо-зеленый", "Çal-ýaşyl", 6 },
                    { 5, "Phantom Black", "Фантомный черный", "Fantom gara", 1 }
                });

            migrationBuilder.InsertData(
                table: "ModelSpecs",
                columns: new[] { "ModelId", "SpecId", "IsNameUse" },
                values: new object[,]
                {
                    { 1, 1, true },
                    { 1, 2, false },
                    { 1, 3, true },
                    { 1, 4, false },
                    { 1, 5, false },
                    { 2, 1, true },
                    { 2, 2, false },
                    { 2, 3, true },
                    { 2, 4, false },
                    { 2, 5, false },
                    { 3, 1, true },
                    { 3, 2, false },
                    { 3, 3, true },
                    { 3, 4, false },
                    { 3, 5, false },
                    { 4, 1, true },
                    { 4, 2, false },
                    { 4, 3, true },
                    { 4, 4, false },
                    { 4, 5, false },
                    { 6, 1, true },
                    { 6, 2, false },
                    { 6, 3, true },
                    { 6, 4, false },
                    { 6, 5, false }
                });

            migrationBuilder.InsertData(
                table: "ProductSpecsValues",
                columns: new[] { "ProductId", "SpecsValueId" },
                values: new object[,]
                {
                    { 22, 1 },
                    { 22, 45 },
                    { 22, 50 }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BrandId", "CategoryId", "IsNew", "IsRecommended", "LineId", "ModelId", "NewPrice", "NotInUse", "OnOrder", "PartNo", "Price", "TypeId", "WarrantyId" },
                values: new object[,]
                {
                    { 1, 1, 11, true, true, 1, 1, null, false, false, null, 440.00m, 1, 2 },
                    { 2, 1, 11, true, true, 1, 1, null, false, false, null, 440.00m, 1, 2 },
                    { 3, 1, 11, false, false, 1, 4, null, false, false, null, 840.00m, 1, 2 },
                    { 4, 1, 11, false, false, 1, 4, null, false, false, null, 840.00m, 1, 2 },
                    { 5, 1, 11, false, false, 1, 4, null, false, false, null, 840.00m, 1, 2 },
                    { 6, 1, 11, false, false, 1, 2, null, false, false, null, 1050.00m, 1, 2 },
                    { 7, 1, 11, false, false, 1, 2, null, false, false, null, 1050.00m, 1, 2 },
                    { 8, 1, 11, false, false, 1, 2, null, false, false, null, 1050.00m, 1, 2 },
                    { 9, 1, 11, false, false, 1, 2, null, false, false, null, 1050.00m, 1, 2 },
                    { 10, 1, 11, false, false, 1, 2, null, false, false, null, 1155.00m, 1, 2 },
                    { 11, 1, 11, false, false, 1, 2, null, false, false, null, 1155.00m, 1, 2 },
                    { 12, 1, 11, false, false, 1, 2, null, false, false, null, 1155.00m, 1, 2 },
                    { 13, 1, 11, false, false, 1, 2, null, false, false, null, 1155.00m, 1, 2 },
                    { 14, 1, 11, false, false, 1, 3, null, false, false, null, 1150.00m, 1, 2 }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BrandId", "CategoryId", "IsNew", "IsRecommended", "LineId", "ModelId", "NewPrice", "NotInUse", "OnOrder", "PartNo", "Price", "TypeId", "WarrantyId" },
                values: new object[,]
                {
                    { 15, 1, 11, false, false, 1, 3, null, false, false, null, 1150.00m, 1, 2 },
                    { 16, 1, 11, false, false, 1, 3, null, false, false, null, 1150.00m, 1, 2 },
                    { 17, 1, 11, false, false, 1, 3, null, false, false, null, 1150.00m, 1, 2 },
                    { 18, 1, 11, false, false, 1, 3, null, false, false, null, 1255.00m, 1, 2 },
                    { 19, 1, 11, false, false, 1, 3, null, false, false, null, 1255.00m, 1, 2 },
                    { 20, 1, 11, false, false, 1, 3, null, false, false, null, 1255.00m, 1, 2 },
                    { 21, 1, 11, false, false, 1, 3, null, false, false, null, 1255.00m, 1, 2 },
                    { 23, 2, 11, true, true, 4, 6, 1600.00m, false, false, null, 1680.00m, 1, 1 },
                    { 24, 2, 11, true, true, 4, 6, 1600.00m, false, false, null, 1680.00m, 1, 1 },
                    { 25, 2, 11, true, true, 4, 6, 1600.00m, false, false, null, 1680.00m, 1, 1 },
                    { 26, 2, 11, true, true, 4, 6, 1500.00m, false, false, null, 1580.00m, 1, 1 },
                    { 27, 2, 11, true, true, 4, 6, 1500.00m, false, false, null, 1580.00m, 1, 1 },
                    { 28, 2, 11, true, true, 4, 6, 1500.00m, false, false, null, 1580.00m, 1, 1 }
                });

            migrationBuilder.InsertData(
                table: "ProductSpecsValueMods",
                columns: new[] { "ProductId", "SpecsValueModId" },
                values: new object[,]
                {
                    { 2, 1 },
                    { 3, 1 },
                    { 4, 2 },
                    { 6, 3 },
                    { 23, 4 },
                    { 24, 5 },
                    { 26, 4 },
                    { 27, 5 }
                });

            migrationBuilder.InsertData(
                table: "ProductSpecsValues",
                columns: new[] { "ProductId", "SpecsValueId" },
                values: new object[,]
                {
                    { 1, 2 },
                    { 1, 12 },
                    { 1, 24 },
                    { 1, 37 },
                    { 1, 48 },
                    { 2, 1 },
                    { 2, 12 },
                    { 2, 24 },
                    { 2, 37 },
                    { 2, 48 },
                    { 3, 1 },
                    { 3, 12 },
                    { 3, 25 },
                    { 3, 41 },
                    { 3, 48 },
                    { 4, 2 },
                    { 4, 12 },
                    { 4, 25 },
                    { 4, 41 },
                    { 4, 48 },
                    { 5, 12 },
                    { 5, 25 },
                    { 5, 41 },
                    { 5, 42 },
                    { 5, 48 },
                    { 6, 6 },
                    { 6, 13 },
                    { 6, 25 },
                    { 6, 43 },
                    { 6, 48 },
                    { 7, 5 },
                    { 7, 13 },
                    { 7, 25 },
                    { 7, 43 }
                });

            migrationBuilder.InsertData(
                table: "ProductSpecsValues",
                columns: new[] { "ProductId", "SpecsValueId" },
                values: new object[,]
                {
                    { 7, 48 },
                    { 8, 9 },
                    { 8, 13 },
                    { 8, 25 },
                    { 8, 43 },
                    { 8, 48 },
                    { 9, 8 },
                    { 9, 13 },
                    { 9, 25 },
                    { 9, 43 },
                    { 9, 48 },
                    { 10, 6 },
                    { 10, 13 },
                    { 10, 26 },
                    { 10, 43 },
                    { 10, 48 },
                    { 11, 5 },
                    { 11, 13 },
                    { 11, 26 },
                    { 11, 43 },
                    { 11, 48 },
                    { 12, 9 },
                    { 12, 13 },
                    { 12, 26 },
                    { 12, 43 },
                    { 12, 48 },
                    { 13, 8 },
                    { 13, 13 },
                    { 13, 26 },
                    { 13, 43 },
                    { 13, 48 },
                    { 14, 6 },
                    { 14, 13 },
                    { 14, 25 },
                    { 14, 44 },
                    { 14, 48 },
                    { 15, 5 },
                    { 15, 13 },
                    { 15, 25 },
                    { 15, 44 },
                    { 15, 48 },
                    { 16, 9 }
                });

            migrationBuilder.InsertData(
                table: "ProductSpecsValues",
                columns: new[] { "ProductId", "SpecsValueId" },
                values: new object[,]
                {
                    { 16, 13 },
                    { 16, 25 },
                    { 16, 44 },
                    { 16, 48 },
                    { 17, 8 },
                    { 17, 13 },
                    { 17, 25 },
                    { 17, 44 },
                    { 17, 48 },
                    { 18, 6 },
                    { 18, 13 },
                    { 18, 26 },
                    { 18, 44 },
                    { 18, 48 },
                    { 19, 5 },
                    { 19, 13 },
                    { 19, 26 },
                    { 19, 44 },
                    { 19, 48 },
                    { 20, 9 },
                    { 20, 13 },
                    { 20, 26 },
                    { 20, 44 },
                    { 20, 48 },
                    { 21, 8 },
                    { 21, 13 },
                    { 21, 26 },
                    { 21, 44 },
                    { 21, 48 },
                    { 23, 6 },
                    { 23, 15 },
                    { 23, 27 },
                    { 23, 48 },
                    { 23, 52 },
                    { 24, 1 },
                    { 24, 15 },
                    { 24, 27 },
                    { 24, 48 },
                    { 24, 52 },
                    { 25, 15 },
                    { 25, 27 },
                    { 25, 48 }
                });

            migrationBuilder.InsertData(
                table: "ProductSpecsValues",
                columns: new[] { "ProductId", "SpecsValueId" },
                values: new object[,]
                {
                    { 25, 52 },
                    { 25, 53 },
                    { 26, 6 },
                    { 26, 15 },
                    { 26, 26 },
                    { 26, 48 },
                    { 26, 52 },
                    { 27, 1 },
                    { 27, 15 },
                    { 27, 26 },
                    { 27, 48 },
                    { 27, 52 },
                    { 28, 15 },
                    { 28, 26 },
                    { 28, 48 },
                    { 28, 52 },
                    { 28, 53 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Brands_Name",
                table: "Brands",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_NameEn",
                table: "Categories",
                column: "NameEn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_NameRu",
                table: "Categories",
                column: "NameRu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_NameTm",
                table: "Categories",
                column: "NameTm",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoryProductAdLinks_ProductId",
                table: "CategoryProductAdLinks",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_CodeName",
                table: "Currencies",
                column: "CodeName",
                unique: true,
                filter: "[CodeName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Login",
                table: "Employees",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PositionId",
                table: "Employees",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CurrencyId",
                table: "Invoices",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_UserId",
                table: "Invoices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Lines_BrandId_Name",
                table: "Lines",
                columns: new[] { "BrandId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Models_BrandId",
                table: "Models",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Models_LineId_Name",
                table: "Models",
                columns: new[] { "LineId", "Name" },
                unique: true,
                filter: "[LineId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ModelSpecs_SpecId",
                table: "ModelSpecs",
                column: "SpecId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_InvoiceId",
                table: "Orders",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProductId",
                table: "Orders",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PurchaseId",
                table: "Orders",
                column: "PurchaseId",
                unique: true,
                filter: "[PurchaseId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_Name",
                table: "Positions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandId",
                table: "Products",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_LineId",
                table: "Products",
                column: "LineId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ModelId",
                table: "Products",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TypeId",
                table: "Products",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_WarrantyId",
                table: "Products",
                column: "WarrantyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecsValueMods_SpecsValueModId",
                table: "ProductSpecsValueMods",
                column: "SpecsValueModId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecsValues_SpecsValueId",
                table: "ProductSpecsValues",
                column: "SpecsValueId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoices_CurrencyId",
                table: "PurchaseInvoices",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoices_SupplierId",
                table: "PurchaseInvoices",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_ProductId",
                table: "Purchases",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_PurchaseInvoiceId",
                table: "Purchases",
                column: "PurchaseInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Specs_NameEn",
                table: "Specs",
                column: "NameEn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Specs_NameRu",
                table: "Specs",
                column: "NameRu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Specs_NameTm",
                table: "Specs",
                column: "NameTm",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpecsValueMods_SpecsValueId",
                table: "SpecsValueMods",
                column: "SpecsValueId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecsValues_SpecId_NameEn",
                table: "SpecsValues",
                columns: new[] { "SpecId", "NameEn" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpecsValues_SpecId_NameRu",
                table: "SpecsValues",
                columns: new[] { "SpecId", "NameRu" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpecsValues_SpecId_NameTm",
                table: "SpecsValues",
                columns: new[] { "SpecId", "NameTm" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Types_NameEn",
                table: "Types",
                column: "NameEn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Types_NameRu",
                table: "Types",
                column: "NameRu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Types_NameTm",
                table: "Types",
                column: "NameTm",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Phone",
                table: "Users",
                column: "Phone",
                unique: true,
                filter: "[Phone] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Warranties_NameEn",
                table: "Warranties",
                column: "NameEn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Warranties_NameRu",
                table: "Warranties",
                column: "NameRu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Warranties_NameTm",
                table: "Warranties",
                column: "NameTm",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wishes_ProductId",
                table: "Wishes",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryAdLinks");

            migrationBuilder.DropTable(
                name: "CategoryProductAdLinks");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Entries");

            migrationBuilder.DropTable(
                name: "ModelSpecs");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "ProductSpecsValueMods");

            migrationBuilder.DropTable(
                name: "ProductSpecsValues");

            migrationBuilder.DropTable(
                name: "Slides");

            migrationBuilder.DropTable(
                name: "Wishes");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "Purchases");

            migrationBuilder.DropTable(
                name: "SpecsValueMods");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "PurchaseInvoices");

            migrationBuilder.DropTable(
                name: "SpecsValues");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Models");

            migrationBuilder.DropTable(
                name: "Types");

            migrationBuilder.DropTable(
                name: "Warranties");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "Specs");

            migrationBuilder.DropTable(
                name: "Lines");

            migrationBuilder.DropTable(
                name: "Brands");
        }
    }
}
