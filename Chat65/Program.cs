using Chat65.Data;
using Chat65.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---- Примусове підключення до Azure SQL ----
var useAzure = true; // <- змінено на true
var connectionString = builder.Configuration.GetConnectionString("AzureSql");

// ---- DbContext ----
builder.Services.AddDbContext<ChatDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    ));

// ---- SignalR ----
builder.Services.AddSignalR()
       .AddAzureSignalR(builder.Configuration["Azure:SignalR:ConnectionString"]);

// ---- MVC ----
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ---- Middleware ----
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// ---- Map Hub ----
app.MapHub<ChatHub>("/ChatHub");

// ---- Map Controllers ----
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();