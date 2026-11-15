
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using RUNE.Data;
using RUNE.Models;
using RUNE.Services.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var versionFile = Path.Combine(builder.Environment.ContentRootPath, "Version.txt");
var versionLine = File.ReadLines(versionFile)
    .FirstOrDefault(line => line.StartsWith("Version:", StringComparison.OrdinalIgnoreCase));

var versionNumber = versionLine?.Split(':', 2).Last().Trim();

if (string.IsNullOrWhiteSpace(versionNumber))
{
    throw new InvalidDataException("Version.txt must contain a 'Version:' line.");
}

var currentVersion = "v" + versionNumber;

var connectionString = builder.Configuration.GetConnectionString("DATABASE_URL");

if (string.IsNullOrWhiteSpace(connectionString))
{
    connectionString = builder.Configuration["DATABASE_URL"];
}

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "PostgreSQL connection string is missing. Set ConnectionStrings:DATABASE_URL or DATABASE_URL.");
}

connectionString = NormalizePostgresConnectionString(connectionString);

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseNpgsql(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<TurkishIdentityErrorDescriber>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();

    const long startupLockId = 0x52554E4553544F52;
    var startupLockAcquired = false;

    await context.Database.OpenConnectionAsync();

    try
    {
        await context.Database.ExecuteSqlRawAsync(
            "SELECT pg_advisory_lock({0})", startupLockId);

        startupLockAcquired = true;

        await context.Database.MigrateAsync();

        // Rolleri oluştur.
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = { "User", "Admin" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var roleResult = await roleManager.CreateAsync(
                    new IdentityRole(role));

                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"'{role}' rolü oluşturulamadı: " +
                        string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }
        }

        // Belirlenen yönetici hesabına Admin rolü ata.
        var adminEmail = builder.Configuration["RUNE_ADMIN_EMAIL"];

        if (!string.IsNullOrWhiteSpace(adminEmail))
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            var adminUser = await userManager.FindByEmailAsync(adminEmail.Trim());

            if (adminUser == null)
            {
                throw new InvalidOperationException(
                    "Admin hesabı bulunamadı. Önce Rune üzerinden kayıt olun.");
            }
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                var result = await userManager.AddToRoleAsync(adminUser, "Admin");

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        "Admin rolü atanamadı: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }

        await GameSeeder.SeedAsync(context);
        await ReleaseNoteSeeder.SeedAsync(context, currentVersion);
    }
    finally
    {
        try
        {
            if (startupLockAcquired)
            {
                await context.Database.ExecuteSqlRawAsync(
                    "SELECT pg_advisory_unlock({0})", startupLockId);
            }
        }
        finally
        {
            await context.Database.CloseConnectionAsync();
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");
    
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

static string NormalizePostgresConnectionString(string value)
{
    if (!Uri.TryCreate(value, UriKind.Absolute, out var databaseUri)
        || (databaseUri.Scheme != "postgres" && databaseUri.Scheme != "postgresql"))
    {
        return value;
    }
    var connectionBuilder = new NpgsqlConnectionStringBuilder
    {
        Host = databaseUri.Host,
        Port = databaseUri.IsDefaultPort || databaseUri.Port < 0
            ? 5432
            : databaseUri.Port,
        Database = Uri.UnescapeDataString(
            databaseUri.AbsolutePath.TrimStart('/'))
    };
    var userInfo = databaseUri.UserInfo.Split(':', 2);
    if (userInfo.Length > 0 && !string.IsNullOrEmpty(userInfo[0]))
    {
        connectionBuilder.Username = Uri.UnescapeDataString(userInfo[0]);
    }
    if (userInfo.Length > 1)
    {
        connectionBuilder.Password = Uri.UnescapeDataString(userInfo[1]);
    }

    foreach (var parameter in databaseUri.Query.TrimStart('?')
        .Split('&', StringSplitOptions.RemoveEmptyEntries))
    {
        var parts = parameter.Split('=', 2);
        var key = Uri.UnescapeDataString(parts[0].Replace('+', ' '));
        var parameterValue = parts.Length > 1
            ? Uri.UnescapeDataString(parts[1].Replace('+', ' '))
            : string.Empty;

        connectionBuilder[key] = parameterValue;
    }

    return connectionBuilder.ConnectionString;
}
