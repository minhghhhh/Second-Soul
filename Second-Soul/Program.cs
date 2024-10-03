using BusssinessObject;
using Repo.CategoryRepo;
using Repo.CouponRepo;
using Repo.GenericRepo;
using Repo.OrderDetailRepo;
using Repo.OrderRepo;
using Repo.ProductRepo;
using Repo.ReviewRepo;
using Repo.UserRepo;
using Service.Mapper;
using Service.UnitOfWork;
using Microsoft.Extensions.Options;
using BusssinessObject.Utils;
using Microsoft.Extensions.Configuration;
using Service.UserService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<SecondSoulShopContext>();
builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IOrderRepo, OrderRepo>();
builder.Services.AddScoped<IOrderDetailRepo, OrderDetailRepo>();   
builder.Services.AddScoped<ICouponRepo, CouponRepo>();
builder.Services.AddScoped<IReviewRepo, ReviewRepo>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOrderRepo, OrderRepo>();
builder.Services.AddScoped<IOrderDetailRepo, OrderDetailRepo>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddMvcCore();  
builder.Services.AddAutoMapper(typeof(MapperConfigurationsProfile));
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

app.MapRazorPages();

app.Run();
