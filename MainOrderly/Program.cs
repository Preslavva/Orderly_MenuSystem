using Models;
using Services;
using MSSQL;
using Microsoft.AspNetCore.Http;

namespace MainOrderly.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<CartServices>();
            builder.Services.AddScoped<MenuItemRepository>();
            builder.Services.AddScoped<NutritionRepository>();
            builder.Services.AddScoped<CartRepository>();
            builder.Services.AddScoped<CheckoutService>();
            builder.Services.AddScoped<KitchenOrderRepository>();
            builder.Services.AddScoped<TableRepository>();
            builder.Services.AddScoped<QRCodeService>();
            builder.Services.AddScoped<KitchenOrderService>();
            builder.Services.AddScoped<MenuService>();
            builder.Services.AddScoped<TableService>();
            builder.Services.AddScoped<NutritionService>();
            builder.Services.AddScoped<TableRepository>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();


            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
