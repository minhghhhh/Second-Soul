using System;
using System.Collections.Generic;
using Data.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Data.Models;

public partial class SecondSoulShopContext : DbContext
{
    public SecondSoulShopContext()
    {
    }

    public SecondSoulShopContext(DbContextOptions<SecondSoulShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Coupon> Coupons { get; set; }

    public virtual DbSet<FavoriteShop> FavoriteShops { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public virtual DbSet<ProductImage> ProductImages { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer(GetConnectionString());
    private string GetConnectionString()
    {
        IConfiguration config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true).Build();
        var strConnection = config.GetConnectionString("DefaultConnection");
        return strConnection;
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACB30732FE");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4E098B892").IsUnique();
            entity.HasIndex(e => e.Email, "UQ__Users__A9D105343B5244F6").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Bankinfo).HasMaxLength(255);
            entity.Property(e => e.Bank).HasMaxLength(50);
            entity.Property(e => e.Bankuser).HasMaxLength(100);
            entity.Property(e => e.Wallet).HasDefaultValue(0);  
            entity.Property(e => e.Role).HasMaxLength(20).HasConversion<string>();
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ImageUrl).HasMaxLength(255).HasDefaultValue(string.Empty);
            entity.Property(e => e.Token).HasMaxLength(255).HasDefaultValue(string.Empty);
            entity.HasMany(u => u.FavoriteShopUsers)
                  .WithOne(fs => fs.User)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.MessageSenders)
                  .WithOne(m => m.Sender)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.Orders)
                  .WithOne(o => o.Customer)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.Reviews)
                  .WithOne(r => r.User)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.ShoppingCarts)
                  .WithOne(sc => sc.User)
                  .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categories__19093A2BC0F6EEE6");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ParentCategoryId).HasColumnName("ParentCategoryID");

            entity.HasOne(d => d.ParentCategory)
                  .WithMany(p => p.InverseParentCategory)
                  .HasForeignKey(d => d.ParentCategoryId)
                  .OnDelete(DeleteBehavior.Restrict) // Ensure correct delete behavior
                  .HasConstraintName("FK__Categories__Parent__3E52440B");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6ED46B279BB");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.SellerId).HasColumnName("SellerID");
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID").IsRequired();
            entity.Property(e => e.Price).HasColumnType("int").IsRequired();
            entity.Property(e => e.SalePrice).HasColumnType("int");
            entity.Property(e => e.Condition)
                  .HasMaxLength(20);
            entity.Property(e => e.Size)
                  .HasMaxLength(5);
            entity.Property(e => e.AddedDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.IsNew).HasDefaultValue(true);
            entity.Property(e => e.IsSale).HasDefaultValue(false);
            entity.Property(e => e.IsReview).HasDefaultValue(false);
            entity.Property(e => e.MainImage).HasMaxLength(255).IsRequired();
            entity.HasOne(d => d.Seller)
                  .WithMany(p => p.Products)
                  .HasForeignKey(d => d.SellerId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK__Products__Seller__44FF419A");

            entity.HasOne(d => d.Category)
                  .WithMany(p => p.Products)
                  .HasForeignKey(d => d.CategoryId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK__Products__Category__45F365D3");

            entity.HasMany(p => p.Reviews)
                  .WithOne(r => r.Product)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(p => p.ShoppingCarts)
                  .WithOne(sc => sc.Product)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ImageUrl).HasMaxLength(255).IsRequired();
            entity.HasOne(d => d.Product)
                  .WithMany(p => p.ProductImages)
                  .HasForeignKey(d => d.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.HasKey(e => e.CouponId).HasName("PK__Coupons__384AF1DA282FDFDE");

            entity.HasIndex(e => e.Code, "UQ__Coupons__A25C5AA7F8D623DA").IsUnique();

            entity.Property(e => e.CouponId).HasColumnName("CouponID");
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.Property(e => e.DiscountPercentage)
                  .HasDefaultValue(0m)
                  .HasColumnType("int");
            entity.Property(e => e.MaxDiscount)
                  .HasDefaultValue(0)
                  .HasColumnType("int");
            entity.Property(e => e.MinSpendAmount)
                  .HasDefaultValue(0)
                  .HasColumnType("int");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAFC6019EBD");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.OrderDate)
                  .HasDefaultValueSql("(getdate())")
                  .HasColumnType("datetime");
            entity.Property(e => e.TotalAmount).HasColumnType("int").IsRequired();
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Descriptions).HasMaxLength(20);
            entity.Property(e => e.FullName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.PhoneNumber).HasMaxLength(15).IsRequired();
            entity.Property(e => e.Address).HasMaxLength(255).IsRequired();
            entity.Property(e => e.CouponId).HasColumnName("CouponID");

            entity.HasOne(d => d.Customer)
                  .WithMany(p => p.Orders)
                  .HasForeignKey(d => d.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK__Orders__Customer__5165187F");

            entity.HasOne(d => d.Coupon)
                  .WithMany(p => p.Orders)
                  .HasForeignKey(d => d.CouponId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("FK__Orders__CouponID__52593CB8");

            entity.HasMany(o => o.Payments)
                  .WithOne(p => p.Order)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D30CFEC7F9C7");

            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID").IsRequired();
            entity.Property(e => e.Price).HasColumnType("int").IsRequired();

            entity.HasOne(d => d.Order)
                  .WithMany(p => p.OrderDetails)
                  .HasForeignKey(d => d.OrderId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK__OrderDeta__Order__5535A963");

            entity.HasOne(d => d.Product)
                  .WithMany(p => p.OrderDetails)
                  .HasForeignKey(d => d.ProductId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK__OrderDeta__Produ__5629CD9C");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A58F4ED0ECF");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID").IsRequired();
            entity.Property(e => e.PaymentDate)
                  .HasDefaultValueSql("(getdate())")
                  .HasColumnType("datetime");
            entity.Property(e => e.Amount).HasColumnType("int").IsRequired();
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.Order)
                  .WithMany(p => p.Payments)
                  .HasForeignKey(d => d.OrderId)
                  .HasConstraintName("FK__Payments__OrderI__6B24EA82");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Messages__C87C037C5E977083");

            entity.Property(e => e.MessageId).HasColumnName("MessageID");
            entity.Property(e => e.SenderId).HasColumnName("SenderID").IsRequired();
            entity.Property(e => e.ReceiverId).HasColumnName("ReceiverID").IsRequired();
            entity.Property(e => e.Subject).HasMaxLength(100);
            entity.Property(e => e.MessageBody).HasColumnType("text").IsRequired();
            entity.Property(e => e.SentDate)
                  .HasDefaultValueSql("(getdate())")
                  .HasColumnType("datetime");
            entity.Property(e => e.IsRead).HasDefaultValue(false).IsRequired();

            entity.HasOne(d => d.Sender)
                  .WithMany(p => p.MessageSenders)
                  .HasForeignKey(d => d.SenderId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK__Messages__Sender__6FE99F9F");

            entity.HasOne(d => d.Receiver)
                  .WithMany(p => p.MessageReceivers)
                  .HasForeignKey(d => d.ReceiverId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK__Messages__Receiv__70DDC3D8");
        });

        modelBuilder.Entity<FavoriteShop>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ShopId }).HasName("PK__Favorite__91F499CE6B53F0C7");

            entity.Property(e => e.UserId).HasColumnName("UserID").IsRequired();
            entity.Property(e => e.ShopId).HasColumnName("ShopID").IsRequired();
            entity.Property(e => e.AddedDate)
                  .HasDefaultValueSql("(getdate())")
                  .HasColumnType("datetime");

            entity.HasOne(d => d.User)
                  .WithMany(p => p.FavoriteShopUsers)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK__FavoriteS__UserI__59FA5E80");

            entity.HasOne(d => d.Shop)
                  .WithMany(p => p.FavoriteShopShops)
                  .HasForeignKey(d => d.ShopId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK__FavoriteS__ShopI__5AEE82B9");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79AEA7568786");

            entity.Property(e => e.ReviewId).HasColumnName("ReviewID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID").IsRequired();
            entity.Property(e => e.UserId).HasColumnName("UserID").IsRequired();
            entity.Property(e => e.Rating).IsRequired();
            entity.Property(e => e.Comment).HasColumnType("text");
            entity.Property(e => e.ReviewDate)
                  .HasDefaultValueSql("(getdate())")
                  .HasColumnType("datetime");

            entity.HasOne(d => d.Product)
                  .WithMany(p => p.Reviews)
                  .HasForeignKey(d => d.ProductId)
                  .HasConstraintName("FK__Reviews__Product__5FB337D6");

            entity.HasOne(d => d.User)
                  .WithMany(p => p.Reviews)
                  .HasForeignKey(d => d.UserId)
                  .HasConstraintName("FK__Reviews__UserID__60A75C0F");
        });

        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ProductId }).HasName("PK__Shopping__51BCD7979CC02949");

            entity.Property(e => e.UserId).HasColumnName("UserID").IsRequired();
            entity.Property(e => e.ProductId).HasColumnName("ProductID").IsRequired();
            entity.Property(e => e.AddedDate)
                  .HasDefaultValueSql("(getdate())")
                  .HasColumnType("datetime");

            entity.HasOne(d => d.User)
                  .WithMany(p => p.ShoppingCarts)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK__ShoppingC__UserI__6477ECF3");

            entity.HasOne(d => d.Product)
                  .WithMany(p => p.ShoppingCarts)
                  .HasForeignKey(d => d.ProductId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK__ShoppingC__Produ__656C112C");
        });

        // Add remaining entities and their configurations as necessary...

        OnModelCreatingPartial(modelBuilder);
    }


    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
