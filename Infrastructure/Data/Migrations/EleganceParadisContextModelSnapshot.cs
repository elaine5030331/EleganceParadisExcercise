﻿// <auto-generated />
using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    [DbContext(typeof(EleganceParadisContext))]
    partial class EleganceParadisContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ApplicationCore.Entities.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Account1")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Account");

                    b.Property<DateTimeOffset>("CreateAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int")
                        .HasComment("EX：黑名單、是否被驗證");

                    b.HasKey("Id");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Cart", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CustomerId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("SpecId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("SpecId");

                    b.ToTable("Carts");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ImageURL");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<int?>("ParentCategoryId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Customer", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Mobile")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Format", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Formats");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("County")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("CreateAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("CustomerId")
                        .HasColumnType("int");

                    b.Property<string>("District")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OrderNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrderStatus")
                        .HasColumnType("int");

                    b.Property<string>("Purchaser")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PurchaserEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PurchaserTel")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("ApplicationCore.Entities.OrderDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal?>("Discount")
                        .HasColumnType("money");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<string>("ProductName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("Sequence")
                        .HasColumnType("int");

                    b.Property<string>("Sku")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("SKU");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("money");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderDetails");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Enable")
                        .HasColumnType("bit")
                        .HasComment("是否上架");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<string>("ProductName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Spu")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("SPU")
                        .HasComment("產品編號");

                    b.HasKey("Id")
                        .HasName("PK_Products_1");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ProductImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("URL");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductImages");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Spec", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<string>("Sku")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("SKU");

                    b.Property<string>("SpecName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasComment("50ml, 75ml, 1 peice");

                    b.Property<int?>("StockQuantity")
                        .HasColumnType("int");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("money");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("Specs");
                });

            modelBuilder.Entity("ProductFormat", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("FormatId")
                        .HasColumnType("int");

                    b.HasKey("ProductId", "FormatId");

                    b.HasIndex("FormatId");

                    b.ToTable("ProductFormats", (string)null);
                });

            modelBuilder.Entity("ApplicationCore.Entities.Cart", b =>
                {
                    b.HasOne("ApplicationCore.Entities.Customer", "Customer")
                        .WithMany("Carts")
                        .HasForeignKey("CustomerId")
                        .IsRequired()
                        .HasConstraintName("FK_Carts_Customers");

                    b.HasOne("ApplicationCore.Entities.Spec", "Spec")
                        .WithMany("Carts")
                        .HasForeignKey("SpecId")
                        .IsRequired()
                        .HasConstraintName("FK_Carts_Specs");

                    b.Navigation("Customer");

                    b.Navigation("Spec");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Category", b =>
                {
                    b.HasOne("ApplicationCore.Entities.Category", "ParentCategory")
                        .WithMany("InverseParentCategory")
                        .HasForeignKey("ParentCategoryId")
                        .HasConstraintName("FK_Categories_Categories");

                    b.Navigation("ParentCategory");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Customer", b =>
                {
                    b.HasOne("ApplicationCore.Entities.Account", "IdNavigation")
                        .WithOne("Customer")
                        .HasForeignKey("ApplicationCore.Entities.Customer", "Id")
                        .IsRequired()
                        .HasConstraintName("FK_Customers_Accounts");

                    b.Navigation("IdNavigation");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Order", b =>
                {
                    b.HasOne("ApplicationCore.Entities.Customer", "Customer")
                        .WithMany("Orders")
                        .HasForeignKey("CustomerId")
                        .IsRequired()
                        .HasConstraintName("FK_Orders_Customers");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("ApplicationCore.Entities.OrderDetail", b =>
                {
                    b.HasOne("ApplicationCore.Entities.Order", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId")
                        .IsRequired()
                        .HasConstraintName("FK_OrderDetails_Orders");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Product", b =>
                {
                    b.HasOne("ApplicationCore.Entities.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .IsRequired()
                        .HasConstraintName("FK_Products_Categories");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ProductImage", b =>
                {
                    b.HasOne("ApplicationCore.Entities.Product", "Product")
                        .WithMany("ProductImages")
                        .HasForeignKey("ProductId")
                        .IsRequired()
                        .HasConstraintName("FK_ProductImages_Products");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Spec", b =>
                {
                    b.HasOne("ApplicationCore.Entities.Product", "Product")
                        .WithMany("Specs")
                        .HasForeignKey("ProductId")
                        .IsRequired()
                        .HasConstraintName("FK_Specs_Products");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ProductFormat", b =>
                {
                    b.HasOne("ApplicationCore.Entities.Format", null)
                        .WithMany()
                        .HasForeignKey("FormatId")
                        .IsRequired()
                        .HasConstraintName("FK_ProductFormats_Formats");

                    b.HasOne("ApplicationCore.Entities.Product", null)
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .IsRequired()
                        .HasConstraintName("FK_ProductFormats_Products");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Account", b =>
                {
                    b.Navigation("Customer");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Category", b =>
                {
                    b.Navigation("InverseParentCategory");

                    b.Navigation("Products");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Customer", b =>
                {
                    b.Navigation("Carts");

                    b.Navigation("Orders");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Order", b =>
                {
                    b.Navigation("OrderDetails");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Product", b =>
                {
                    b.Navigation("ProductImages");

                    b.Navigation("Specs");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Spec", b =>
                {
                    b.Navigation("Carts");
                });
#pragma warning restore 612, 618
        }
    }
}
