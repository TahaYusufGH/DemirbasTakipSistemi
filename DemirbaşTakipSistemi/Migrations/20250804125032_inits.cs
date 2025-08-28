using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemirbaşTakipSistemi.Migrations
{
    /// <inheritdoc />
    public partial class inits : Migration
    {
        /// <inheritdoc />
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
                    Ad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Departman = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SicilNo = table.Column<string>(type: "nvarchar(450)", nullable: true),
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
                name: "SarfMalzemeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MalzemeKodu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Birim = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Kategori = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Marka = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MinimumStok = table.Column<int>(type: "int", nullable: false),
                    MevcutStok = table.Column<int>(type: "int", nullable: false),
                    SonGuncelleme = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SarfMalzemeler", x => x.Id);
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
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
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
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
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
                name: "Odalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OdaKodu = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kat = table.Column<int>(type: "int", nullable: false),
                    Bina = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SorumluPersonelId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Odalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Odalar_AspNetUsers_SorumluPersonelId",
                        column: x => x.SorumluPersonelId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DepoIslemler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SarfMalzemeId = table.Column<int>(type: "int", nullable: false),
                    IslemTipi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IslemTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Miktar = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IslemYapanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BelgeNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tedarikci = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TalepEdenId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OnayDurumu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OnaylayanId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OnayTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RedNedeni = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepoIslemler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepoIslemler_AspNetUsers_IslemYapanId",
                        column: x => x.IslemYapanId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepoIslemler_AspNetUsers_OnaylayanId",
                        column: x => x.OnaylayanId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepoIslemler_AspNetUsers_TalepEdenId",
                        column: x => x.TalepEdenId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepoIslemler_SarfMalzemeler_SarfMalzemeId",
                        column: x => x.SarfMalzemeId,
                        principalTable: "SarfMalzemeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Demirbaslar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DemirbasKodu = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kategori = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Marka = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlimTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Durum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeriNo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OdaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demirbaslar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Demirbaslar_Odalar_OdaId",
                        column: x => x.OdaId,
                        principalTable: "Odalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ArizaKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DemirbasId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArizaKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArizaKayitlari_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArizaKayitlari_Demirbaslar_DemirbasId",
                        column: x => x.DemirbasId,
                        principalTable: "Demirbaslar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Arizalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DemirbasId = table.Column<int>(type: "int", nullable: false),
                    BildirenId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ArizaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Cozum = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Durum = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CozumTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CozenPersonelId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arizalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Arizalar_AspNetUsers_BildirenId",
                        column: x => x.BildirenId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Arizalar_AspNetUsers_CozenPersonelId",
                        column: x => x.CozenPersonelId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Arizalar_Demirbaslar_DemirbasId",
                        column: x => x.DemirbasId,
                        principalTable: "Demirbaslar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bakimlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DemirbasId = table.Column<int>(type: "int", nullable: false),
                    BakimTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BakimTuru = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YapilanIslemler = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Durum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TamamlanmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BakimYapanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SonrakiBakimTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bakimlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bakimlar_AspNetUsers_BakimYapanId",
                        column: x => x.BakimYapanId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bakimlar_Demirbaslar_DemirbasId",
                        column: x => x.DemirbasId,
                        principalTable: "Demirbaslar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Zimmetler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonelId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DemirbasId = table.Column<int>(type: "int", nullable: false),
                    ZimmetTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IadeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsAktif = table.Column<bool>(type: "bit", nullable: false),
                    TeslimEdenId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TeslimAlanId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zimmetler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zimmetler_AspNetUsers_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Zimmetler_AspNetUsers_TeslimAlanId",
                        column: x => x.TeslimAlanId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Zimmetler_AspNetUsers_TeslimEdenId",
                        column: x => x.TeslimEdenId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Zimmetler_Demirbaslar_DemirbasId",
                        column: x => x.DemirbasId,
                        principalTable: "Demirbaslar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArizaKayitlari_DemirbasId",
                table: "ArizaKayitlari",
                column: "DemirbasId");

            migrationBuilder.CreateIndex(
                name: "IX_ArizaKayitlari_UserId",
                table: "ArizaKayitlari",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Arizalar_ArizaTarihi",
                table: "Arizalar",
                column: "ArizaTarihi");

            migrationBuilder.CreateIndex(
                name: "IX_Arizalar_BildirenId",
                table: "Arizalar",
                column: "BildirenId");

            migrationBuilder.CreateIndex(
                name: "IX_Arizalar_CozenPersonelId",
                table: "Arizalar",
                column: "CozenPersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_Arizalar_DemirbasId",
                table: "Arizalar",
                column: "DemirbasId");

            migrationBuilder.CreateIndex(
                name: "IX_Arizalar_Durum",
                table: "Arizalar",
                column: "Durum");

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
                name: "IX_AspNetUsers_SicilNo",
                table: "AspNetUsers",
                column: "SicilNo");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Bakimlar_BakimYapanId",
                table: "Bakimlar",
                column: "BakimYapanId");

            migrationBuilder.CreateIndex(
                name: "IX_Bakimlar_DemirbasId",
                table: "Bakimlar",
                column: "DemirbasId");

            migrationBuilder.CreateIndex(
                name: "IX_Demirbaslar_DemirbasKodu",
                table: "Demirbaslar",
                column: "DemirbasKodu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Demirbaslar_OdaId",
                table: "Demirbaslar",
                column: "OdaId");

            migrationBuilder.CreateIndex(
                name: "IX_Demirbaslar_SeriNo",
                table: "Demirbaslar",
                column: "SeriNo");

            migrationBuilder.CreateIndex(
                name: "IX_DepoIslemler_IslemTarihi",
                table: "DepoIslemler",
                column: "IslemTarihi");

            migrationBuilder.CreateIndex(
                name: "IX_DepoIslemler_IslemYapanId",
                table: "DepoIslemler",
                column: "IslemYapanId");

            migrationBuilder.CreateIndex(
                name: "IX_DepoIslemler_OnaylayanId",
                table: "DepoIslemler",
                column: "OnaylayanId");

            migrationBuilder.CreateIndex(
                name: "IX_DepoIslemler_SarfMalzemeId",
                table: "DepoIslemler",
                column: "SarfMalzemeId");

            migrationBuilder.CreateIndex(
                name: "IX_DepoIslemler_TalepEdenId",
                table: "DepoIslemler",
                column: "TalepEdenId");

            migrationBuilder.CreateIndex(
                name: "IX_Odalar_OdaKodu",
                table: "Odalar",
                column: "OdaKodu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Odalar_SorumluPersonelId",
                table: "Odalar",
                column: "SorumluPersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_SarfMalzemeler_MalzemeKodu",
                table: "SarfMalzemeler",
                column: "MalzemeKodu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Zimmetler_DemirbasId_IsAktif",
                table: "Zimmetler",
                columns: new[] { "DemirbasId", "IsAktif" },
                filter: "[IsAktif] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Zimmetler_PersonelId",
                table: "Zimmetler",
                column: "PersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_Zimmetler_TeslimAlanId",
                table: "Zimmetler",
                column: "TeslimAlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Zimmetler_TeslimEdenId",
                table: "Zimmetler",
                column: "TeslimEdenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArizaKayitlari");

            migrationBuilder.DropTable(
                name: "Arizalar");

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
                name: "Bakimlar");

            migrationBuilder.DropTable(
                name: "DepoIslemler");

            migrationBuilder.DropTable(
                name: "Zimmetler");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "SarfMalzemeler");

            migrationBuilder.DropTable(
                name: "Demirbaslar");

            migrationBuilder.DropTable(
                name: "Odalar");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
