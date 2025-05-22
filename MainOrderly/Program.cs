using MainOrderly.WebApp.Helpers;
using Models;
using Services;
using MSSQL;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;

namespace MainOrderly.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddHttpContextAccessor();

            // Existing services
            builder.Services.AddScoped<CartService>();
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
            builder.Services.AddScoped<AllergenRepository>();
            builder.Services.AddScoped<AllergenService>();
            builder.Services.AddScoped<OrderHistoryRepository>();
            builder.Services.AddScoped<HistoryService>();

            builder.Services.AddScoped<IngredientService>();
            builder.Services.AddScoped<IngredientRepository>();
            builder.Services.AddScoped<TimerHelpers>();
            builder.Services.AddScoped<StaffService>();
            builder.Services.AddScoped<RestaurantRepository>();
            builder.Services.AddScoped<RestaurantService>();

            // New services for authentication
            builder.Services.AddScoped<OwnerRepository>();
            builder.Services.AddScoped<StaffRepository>();
            builder.Services.AddScoped<RoleRepository>();
            builder.Services.AddScoped<AuthenticationService>();

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