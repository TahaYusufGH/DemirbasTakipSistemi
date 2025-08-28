using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DemirbaşTakipSistemi.Data;
using DemirbaşTakipSistemi.Models;
using DemirbaşTakipSistemi.Models.Enums;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Set Turkish culture for application
var cultureInfo = new CultureInfo("tr-TR");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    }));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    
    // Disable registration
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Disable registration path
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Configure Identity Server to use the same JWT authentication
builder.Services.Configure<IdentityOptions>(options =>
{
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
});

builder.Services.AddRazorPages(options =>
{
    // Disable registration page
    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Register");
    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Register");
});

builder.Services.AddControllersWithViews();

// Configure cookie policy
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

// Add authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequirePersonelRole", policy => policy.RequireRole("Personel"));
    options.AddPolicy("RequireBilgiIslemRole", policy => policy.RequireRole("BilgiIslem"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Seed initial data
await SeedDatabase(app);

// Run the application
app.Run();

// Seed database method
async Task SeedDatabase(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var logger = services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Ensuring database is created and migrated...");
            
            // Ensure database is migrated
            await context.Database.MigrateAsync();

            // Create roles if they don't exist
            logger.LogInformation("Creating roles...");
            string[] roleNames = { "Admin", "Personel", "BilgiIslem" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    logger.LogInformation($"Creating role: {roleName}");
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create default admin user if it doesn't exist
            logger.LogInformation("Creating admin user...");
            var adminEmail = "admin@adalet.gov.tr";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Ad = "Sistem",
                    Soyad = "Yöneticisi",
                    Departman = "Bilgi İşlem Müdürlüğü",
                    SicilNo = "A0001",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    logger.LogInformation("Admin user created successfully");
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    logger.LogError("Failed to create admin user: {0}", string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            // Personel kullanıcıları ekle
            logger.LogInformation("Creating personnel users...");
            var personelUsers = new List<(string email, string password, string ad, string soyad, string departman, string sicilNo)>
            {
                ("personel1@adalet.gov.tr", "Personel1!", "Ahmet", "Yılmaz", "İcra Dairesi", "P0001"),
                ("personel2@adalet.gov.tr", "Personel2!", "Ayşe", "Kaya", "Mahkeme Kalemi", "P0002"),
                ("personel3@adalet.gov.tr", "Personel3!", "Mehmet", "Demir", "İdari İşler", "P0003")
            };

            foreach (var personel in personelUsers)
            {
                if (await userManager.FindByEmailAsync(personel.email) == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = personel.email,
                        Email = personel.email,
                        Ad = personel.ad,
                        Soyad = personel.soyad,
                        Departman = personel.departman,
                        SicilNo = personel.sicilNo,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, personel.password);
                    if (result.Succeeded)
                    {
                        logger.LogInformation($"Personnel user created: {personel.email}");
                        await userManager.AddToRoleAsync(user, "Personel");
                    }
                    else
                    {
                        logger.LogError("Failed to create personnel user {0}: {1}", personel.email, 
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }

            // Bilgi İşlem kullanıcıları ekle
            logger.LogInformation("Creating IT department users...");
            var bilgiIslemUsers = new List<(string email, string password, string ad, string soyad, string sicilNo)>
            {
                ("bilgiislem1@adalet.gov.tr", "BilgiIslem1!", "Emre", "Şahin", "B0001"),
                ("bilgiislem2@adalet.gov.tr", "BilgiIslem2!", "Zeynep", "Yıldız", "B0002")
            };

            foreach (var bilgiIslem in bilgiIslemUsers)
            {
                if (await userManager.FindByEmailAsync(bilgiIslem.email) == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = bilgiIslem.email,
                        Email = bilgiIslem.email,
                        Ad = bilgiIslem.ad,
                        Soyad = bilgiIslem.soyad,
                        Departman = "Bilgi İşlem Müdürlüğü",
                        SicilNo = bilgiIslem.sicilNo,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, bilgiIslem.password);
                    if (result.Succeeded)
                    {
                        logger.LogInformation($"IT user created: {bilgiIslem.email}");
                        await userManager.AddToRoleAsync(user, "BilgiIslem");
                    }
                    else
                    {
                        logger.LogError("Failed to create IT user {0}: {1}", bilgiIslem.email,
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }

            // Seed some rooms if none exist
            if (!context.Odalar.Any())
            {
                logger.LogInformation("Creating sample rooms...");
                var odalar = new List<Oda>
                {
                    new Oda { OdaKodu = "A101", Ad = "Bilgi İşlem Odası", Kat = 1, Bina = "A Blok" },
                    new Oda { OdaKodu = "A102", Ad = "İdari İşler Odası", Kat = 1, Bina = "A Blok" },
                    new Oda { OdaKodu = "B201", Ad = "Mahkeme Kalemi", Kat = 2, Bina = "B Blok" },
                    new Oda { OdaKodu = "B202", Ad = "İcra Dairesi", Kat = 2, Bina = "B Blok" }
                };

                context.Odalar.AddRange(odalar);
                await context.SaveChangesAsync();
                logger.LogInformation("Sample rooms created successfully");
            }
            
            logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}


