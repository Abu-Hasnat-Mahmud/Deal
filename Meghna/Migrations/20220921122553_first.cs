using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Deal.Migrations
{
    public partial class first : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    CurrId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.CurrId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    ClientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.ClientId);
                    table.ForeignKey(
                        name: "FK_Client_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PayType",
                columns: table => new
                {
                    PayTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayTypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsCustom = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayType", x => x.PayTypeId);
                    table.ForeignKey(
                        name: "FK_PayType_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "S_CheckoutMaster",
                columns: table => new
                {
                    MasterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Key = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsCustomPayment = table.Column<bool>(type: "bit", nullable: false),
                    IsCallbackURL = table.Column<bool>(type: "bit", nullable: false),
                    CallbackUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_S_CheckoutMaster", x => x.MasterId);
                    table.ForeignKey(
                        name: "FK_S_CheckoutMaster_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "S_QRInfo",
                columns: table => new
                {
                    QRId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Key = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_S_QRInfo", x => x.QRId);
                    table.ForeignKey(
                        name: "FK_S_QRInfo_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CurrId = table.Column<int>(type: "int", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Product_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_Currency_CurrId",
                        column: x => x.CurrId,
                        principalTable: "Currency",
                        principalColumn: "CurrId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "S_PaymentLink",
                columns: table => new
                {
                    LinkId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    CurrId = table.Column<int>(type: "int", nullable: false),
                    IsInactive = table.Column<bool>(type: "bit", nullable: false),
                    CustomLink = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Key = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsCustomPayment = table.Column<bool>(type: "bit", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_S_PaymentLink", x => x.LinkId);
                    table.ForeignKey(
                        name: "FK_S_PaymentLink_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_S_PaymentLink_Currency_CurrId",
                        column: x => x.CurrId,
                        principalTable: "Currency",
                        principalColumn: "CurrId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransactionClearanceMaster",
                columns: table => new
                {
                    MasterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserIdFor = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionClearanceMaster", x => x.MasterId);
                    table.ForeignKey(
                        name: "FK_TransactionClearanceMaster_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransactionClearanceMaster_AspNetUsers_UserIdFor",
                        column: x => x.UserIdFor,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransactionClearanceMaster_Currency_CurrId",
                        column: x => x.CurrId,
                        principalTable: "Currency",
                        principalColumn: "CurrId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPayTypeControl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayTypeId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsUsing = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPayTypeControl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPayTypeControl_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPayTypeControl_PayType_PayTypeId",
                        column: x => x.PayTypeId,
                        principalTable: "PayType",
                        principalColumn: "PayTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "S_CheckoutDetails",
                columns: table => new
                {
                    DetaileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    MasterId = table.Column<int>(type: "int", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_S_CheckoutDetails", x => x.DetaileId);
                    table.ForeignKey(
                        name: "FK_S_CheckoutDetails_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_S_CheckoutDetails_S_CheckoutMaster_MasterId",
                        column: x => x.MasterId,
                        principalTable: "S_CheckoutMaster",
                        principalColumn: "MasterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransaction",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Order_Id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Payment_Ref_Id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrId = table.Column<int>(type: "int", nullable: false),
                    PayTypeId = table.Column<int>(type: "int", nullable: true),
                    PaymentLinkId = table.Column<int>(type: "int", nullable: true),
                    CheckoutId = table.Column<int>(type: "int", nullable: true),
                    QRId = table.Column<int>(type: "int", nullable: true),
                    IsCustomPayment = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    IsClientUsed = table.Column<bool>(type: "bit", nullable: false),
                    UserIdFor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClearTrans = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransaction", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_PaymentTransaction_Currency_CurrId",
                        column: x => x.CurrId,
                        principalTable: "Currency",
                        principalColumn: "CurrId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentTransaction_PayType_PayTypeId",
                        column: x => x.PayTypeId,
                        principalTable: "PayType",
                        principalColumn: "PayTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentTransaction_S_CheckoutMaster_CheckoutId",
                        column: x => x.CheckoutId,
                        principalTable: "S_CheckoutMaster",
                        principalColumn: "MasterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentTransaction_S_PaymentLink_PaymentLinkId",
                        column: x => x.PaymentLinkId,
                        principalTable: "S_PaymentLink",
                        principalColumn: "LinkId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentTransaction_S_QRInfo_QRId",
                        column: x => x.QRId,
                        principalTable: "S_QRInfo",
                        principalColumn: "QRId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomPayment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AmountPaid = table.Column<double>(type: "float", nullable: false),
                    CurrId = table.Column<int>(type: "int", nullable: false),
                    IsAccept = table.Column<bool>(type: "bit", nullable: false),
                    IsReject = table.Column<bool>(type: "bit", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserIdFor = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomPayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomPayment_AspNetUsers_UserIdFor",
                        column: x => x.UserIdFor,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomPayment_Currency_CurrId",
                        column: x => x.CurrId,
                        principalTable: "Currency",
                        principalColumn: "CurrId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomPayment_PaymentTransaction_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "PaymentTransaction",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransactionClearanceDetails",
                columns: table => new
                {
                    DetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MasterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionClearanceDetails", x => x.DetailsId);
                    table.ForeignKey(
                        name: "FK_TransactionClearanceDetails_PaymentTransaction_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "PaymentTransaction",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransactionClearanceDetails_TransactionClearanceMaster_MasterId",
                        column: x => x.MasterId,
                        principalTable: "TransactionClearanceMaster",
                        principalColumn: "MasterId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Client_UserId",
                table: "Client",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomPayment_CurrId",
                table: "CustomPayment",
                column: "CurrId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomPayment_TransactionId",
                table: "CustomPayment",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomPayment_UserIdFor",
                table: "CustomPayment",
                column: "UserIdFor");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransaction_CheckoutId",
                table: "PaymentTransaction",
                column: "CheckoutId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransaction_CurrId",
                table: "PaymentTransaction",
                column: "CurrId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransaction_PaymentLinkId",
                table: "PaymentTransaction",
                column: "PaymentLinkId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransaction_PayTypeId",
                table: "PaymentTransaction",
                column: "PayTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransaction_QRId",
                table: "PaymentTransaction",
                column: "QRId");

            migrationBuilder.CreateIndex(
                name: "IX_PayType_UserId",
                table: "PayType",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CurrId",
                table: "Product",
                column: "CurrId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_UserId",
                table: "Product",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_S_CheckoutDetails_MasterId",
                table: "S_CheckoutDetails",
                column: "MasterId");

            migrationBuilder.CreateIndex(
                name: "IX_S_CheckoutDetails_ProductId",
                table: "S_CheckoutDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_S_CheckoutMaster_UserId",
                table: "S_CheckoutMaster",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_S_PaymentLink_CurrId",
                table: "S_PaymentLink",
                column: "CurrId");

            migrationBuilder.CreateIndex(
                name: "IX_S_PaymentLink_UserId",
                table: "S_PaymentLink",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_S_QRInfo_UserId",
                table: "S_QRInfo",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionClearanceDetails_MasterId",
                table: "TransactionClearanceDetails",
                column: "MasterId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionClearanceDetails_TransactionId",
                table: "TransactionClearanceDetails",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionClearanceMaster_CurrId",
                table: "TransactionClearanceMaster",
                column: "CurrId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionClearanceMaster_UserId",
                table: "TransactionClearanceMaster",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionClearanceMaster_UserIdFor",
                table: "TransactionClearanceMaster",
                column: "UserIdFor");

            migrationBuilder.CreateIndex(
                name: "IX_UserPayTypeControl_PayTypeId",
                table: "UserPayTypeControl",
                column: "PayTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPayTypeControl_UserId",
                table: "UserPayTypeControl",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropTable(
                name: "CustomPayment");

            migrationBuilder.DropTable(
                name: "S_CheckoutDetails");

            migrationBuilder.DropTable(
                name: "TransactionClearanceDetails");

            migrationBuilder.DropTable(
                name: "UserPayTypeControl");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "PaymentTransaction");

            migrationBuilder.DropTable(
                name: "TransactionClearanceMaster");

            migrationBuilder.DropTable(
                name: "PayType");

            migrationBuilder.DropTable(
                name: "S_CheckoutMaster");

            migrationBuilder.DropTable(
                name: "S_PaymentLink");

            migrationBuilder.DropTable(
                name: "S_QRInfo");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
