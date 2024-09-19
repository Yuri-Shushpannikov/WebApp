using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Razor;
using WebApp.Backend.Data;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);


// Настройка Entity Framework для PostgreSQL
builder.Services.AddDbContext<WebAppContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("WebAppContext") ?? throw new InvalidOperationException("Connection string 'WebAppContext' not found.")));

// MVC-контроллеры и новый путь к представлениям
builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        options.ViewLocationFormats.Add("/Frontend/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
        options.ViewLocationFormats.Add("/Frontend/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Перенаправление HTTP запросов на HTTPS
app.UseHttpsRedirection();
app.UseStaticFiles();
// Маршрутизация
app.UseRouting();

app.UseAuthorization();

//Маршрут по умолчанию
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//Открытие браузера с помощью .exe
Process.Start(new ProcessStartInfo("cmd", "/c start http://localhost:5000") { CreateNoWindow = true });

app.Run();
