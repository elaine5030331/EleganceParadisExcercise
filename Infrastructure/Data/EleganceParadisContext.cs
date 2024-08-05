using System;
using System.Collections.Generic;
using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public partial class EleganceParadisContext : DbContext
{
    public EleganceParadisContext()
    {
    }

    public EleganceParadisContext(DbContextOptions<EleganceParadisContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AuthTokenHistory> AuthTokenHistories { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Format> Formats { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<Spec> Specs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.Property(e => e.Mobile).HasMaxLength(256);
            entity.Property(e => e.Status).HasComment("EX：黑名單、是否被驗證");
        });

        modelBuilder.Entity<AuthTokenHistory>(entity =>
        {
            entity.Property(e => e.AccessToken).HasComment("JWT");
            entity.Property(e => e.ExpiredTime).HasComment("RefreshToken過期時間");

            entity.HasOne(d => d.Account).WithMany(p => p.AuthTokenHistories)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuthTokenHistories_Accounts");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasOne(d => d.Account).WithMany(p => p.Carts)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Carts_Accounts");

            entity.HasOne(d => d.Spec).WithMany(p => p.Carts)
                .HasForeignKey(d => d.SpecId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Carts_Specs");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.ImageUrl).HasColumnName("ImageURL");

            entity.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory)
                .HasForeignKey(d => d.ParentCategoryId)
                .HasConstraintName("FK_Categories_Categories");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.PaymentType).HasComment("0 = LinePay, 1 = ECPay");
            entity.Property(e => e.Purchaser).HasMaxLength(256);
            entity.Property(e => e.PurchaserTel).HasMaxLength(50);

            entity.HasOne(d => d.Account).WithMany(p => p.Orders)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Accounts");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.Property(e => e.Discount).HasColumnType("money");
            entity.Property(e => e.Sku).HasColumnName("SKU");
            entity.Property(e => e.UnitPrice).HasColumnType("money");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Orders");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Products_1");

            entity.Property(e => e.Enable).HasComment("是否上架");
            entity.Property(e => e.Spu)
                .HasComment("產品編號")
                .HasColumnName("SPU");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Categories");

            entity.HasMany(d => d.Formats).WithMany(p => p.Products)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductFormat",
                    r => r.HasOne<Format>().WithMany()
                        .HasForeignKey("FormatId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ProductFormats_Formats"),
                    l => l.HasOne<Product>().WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ProductFormats_Products"),
                    j =>
                    {
                        j.HasKey("ProductId", "FormatId");
                        j.ToTable("ProductFormats");
                    });
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.Property(e => e.Url).HasColumnName("URL");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductImages_Products");
        });

        modelBuilder.Entity<Spec>(entity =>
        {
            entity.Property(e => e.Sku).HasColumnName("SKU");
            entity.Property(e => e.SpecName)
                .HasMaxLength(256)
                .HasComment("50ml, 75ml, 1 peice");
            entity.Property(e => e.UnitPrice).HasColumnType("money");

            entity.HasOne(d => d.Product).WithMany(p => p.Specs)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Specs_Products");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
