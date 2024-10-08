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
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.Name = "SecondSoul";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
});

/*builder.Configuration.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
var configuration = builder.Configuration;


var payOs = new PayOS(configuration["Environment:PAYOS_CLIENT_ID"] ?? throw new Exception("Cannot find environment client"),
                    configuration["Environment:PAYOS_API_KEY"] ?? throw new Exception("Cannot find environment api"),
                    configuration["Environment:PAYOS_CHECKSUM_KEY"] ?? throw new Exception("Cannot find environment sum"));
builder.Services.AddScoped<PayOS>(_ => payOs);*/

var cloudinarySettings = builder.Configuration.GetSection("CloudinaryOptions").Get<CloudinaryOptions>();

// Create Cloudinary account
Account account = new Account(
    cloudinarySettings.CloudName,
    cloudinarySettings.ApiKey,
    cloudinarySettings.ApiSecret
);
builder.Services.AddHttpContextAccessor();
builder.Services.AddHostedService<BackGround>(); // Add the background service
Cloudinary cloudinary = new Cloudinary(account);
builder.Services.AddSingleton(cloudinary);
builder.Services.AddDbContext<SecondSoulShopContext>();
builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
builder.Services.AddScoped<ICouponBusiness, CouponBusiness>();
builder.Services.AddScoped<IUserBusiness, UserBusiness>();
builder.Services.AddScoped<IProductImageBusiness, ProductImageBusiness>();
builder.Services.AddScoped<ICategoryBusiness, CategoryBusiness>();
builder.Services.AddScoped<IProductBusiness, ProductBusiness>();
builder.Services.AddScoped<IOrderBusiness, OrderBusiness>();
builder.Services.AddScoped<IOrderDetailBusiness, OrderDetailBusiness>();
builder.Services.AddScoped<IPaymentBusiness, PaymentBusiness>();
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
builder.Services.AddMvcCore();
builder.Services.AddRazorPages();
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
