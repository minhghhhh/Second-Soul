using BusinessObject;
using BusssinessObject;
using Data;
using Data.Base;
using Data.Models;
using Data.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddDbContext<SecondSoulShopContext>();
builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
builder.Services.AddScoped<IUserBusiness,UserBusiness>();
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
builder.Services.AddScoped<ReviewRepo>();
builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddScoped<OrderRepo>();
builder.Services.AddScoped<OrderDetailRepo>();
builder.Services.AddMvcCore();  
builder.Services.AddRazorPages();
//builder.Services.Configure<CloudinaryOptions>(Configuration.GetSection("Cloudinary"));
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
*/app.MapRazorPages();

app.Run();
/*public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path;
        if (!path.StartsWithSegments("/Login") && !context.Session.Keys.Contains("AccountId"))
        {
            context.Response.Redirect("/Login");
            return;
        }

        await _next(context);
    }
}*/