using BusinessObject;
using BusssinessObject;
using CloudinaryDotNet;
using Microsoft.Extensions.DependencyInjection;
using Data;
using Data.Base;
using Data.Models;
using Data.Repository;
using Microsoft.Extensions.Configuration;
using Data.Utils;
using Second_Soul;
using FluentAssertions.Common;
using Net.payOS;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

PayOS payOS = new PayOS(configuration["Environment:PAYOS_CLIENT_ID"] ?? throw new Exception("Cannot find environment"),
                    configuration["Environment:PAYOS_API_KEY"] ?? throw new Exception("Cannot find environment"),
                    configuration["Environment:PAYOS_CHECKSUM_KEY"] ?? throw new Exception("Cannot find environment"));
builder.Services.AddSingleton(payOS);
builder.Services.AddControllers();
var cloudinarySettings = builder.Configuration.GetSection("CloudinaryOptions").Get<CloudinaryOptions>();

// Create Cloudinary account
Account account = new Account(
    cloudinarySettings.CloudName,
    cloudinarySettings.ApiKey,
    cloudinarySettings.ApiSecret
);
builder.Services.AddControllers(); // This enables API controller
builder.Services.AddHttpContextAccessor();
builder.Services.AddHostedService<BackGround>(); // Add the background service
Cloudinary cloudinary = new Cloudinary(account);
builder.Services.AddSingleton(cloudinary);
builder.Services.AddDbContext<SecondSoulShopContext>();
builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
builder.Services.AddScoped<ICouponBusiness, CouponBusiness>();
builder.Services.AddScoped<IUserBusiness, UserBusiness>();
builder.Services.AddScoped<IFavoriteShopBusiness, FavoriteShopBusiness>();
builder.Services.AddScoped<IProductImageBusiness, ProductImageBusiness>();
builder.Services.AddScoped<ICategoryBusiness, CategoryBusiness>();
builder.Services.AddScoped<IProductBusiness, ProductBusiness>();
builder.Services.AddScoped<IOrderBusiness, OrderBusiness>();
builder.Services.AddScoped<IOrderDetailBusiness, OrderDetailBusiness>();
builder.Services.AddScoped<IPaymentBusiness, PaymentBusiness>();
builder.Services.AddScoped<IShoppingCartBusiness, ShoppingCartBusiness>();
builder.Services.AddScoped<ProductRepo>();
builder.Services.AddScoped<PaymentRepo>();
builder.Services.AddScoped<CategoryRepo>();
builder.Services.AddScoped<UserRepo>();
builder.Services.AddScoped<OrderRepo>();
builder.Services.AddScoped<OrderDetailRepo>();
builder.Services.AddScoped<CouponRepo>();
builder.Services.AddScoped<ProductImageRepo>();
builder.Services.AddScoped<ReviewRepo>();
builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddScoped<OrderRepo>();
builder.Services.AddScoped<OrderDetailRepo>();
builder.Services.AddScoped<ShoppingCartRepo>();
builder.Services.AddScoped<FavoriteShopsRepo>();
builder.Services.AddMvcCore();

builder.Services.AddRazorPages();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set timeout for session
    //options.Cookie.HttpOnly = true; // Cookie settings
    options.Cookie.IsEssential = true; // Make the session cookie essential
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.Name = "SecondSoul";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
/*app.UseMiddleware<AuthenticationMiddleware>();
*/
app.MapRazorPages();

app.Run();
