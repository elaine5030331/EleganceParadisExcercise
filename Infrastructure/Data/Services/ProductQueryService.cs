using ApplicationCore.DTOs.ProductDTOs;
using Dapper;
using EleganceParadisAPI.Services;
using System.Data;

namespace Infrastructure.Data.Services
{
    public class ProductQueryService : IProductQueryService
    {
        private readonly IDbConnection _connection;

        public ProductQueryService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<GetProductListDTO>> GetProducts(int categoryId)
        {
            var parameter = new { CategoryId = categoryId };
            var sql = @"WITH MinUnitPrice_CTE(ProductId, MinUnitPrice)
                        AS(
	                        SELECT
		                        ProductId,
		                        MIN(UnitPrice)
	                        FROM Specs
	                        GROUP BY ProductId
                        )
                        SELECT 
                            Products.Id AS ProductId,
                            Categories.Name AS CategoryName,
                            ProductName,
                            MinUnitPrice_CTE.MinUnitPrice AS UnitPrice,
                            ProductImages.URL AS ProductImageUrl
                        FROM Products 
                        JOIN MinUnitPrice_CTE ON MinUnitPrice_CTE.ProductId = Products.Id
                        JOIN Categories ON Categories.Id = Products.CategoryId 
                        left JOIN ProductImages ON Products.Id = ProductImages.ProductId 
                        WHERE Categories.Id = @CategoryId
                        AND ProductImages.[Order] = 1
                        ORDER BY Categories.[Order]";

            return (await _connection.QueryAsync<GetProductListDTO>(sql, parameter)).ToList();
        }

        public async Task<ProductDTO> GetProductById(int productId)
        {
            var parameter = new { ProductId = productId };
            var sql = @"
                        SELECT 
	                        Products.Id AS ProductId,
	                        Categories.Name AS CategoryName,
	                        ProductName,
	                        Products.SPU AS SPU,
	                        Products.Description AS Description,
	                        Specs.Id AS SpecId,
	                        Specs.SKU AS SKU,
	                        Specs.UnitPrice AS UnitPrice,
	                        Specs.SpecName AS SpecName,
	                        Specs.[Order] AS SpecOrder,
	                        Specs.StockQuantity AS StockQuantity,
	                        ProductImages.Id AS ProductImageId,
	                        ProductImages.URL AS ProductImageUrl,
	                        ProductImages.[Order] AS ProductImageOrder
                        FROM Products
                        JOIN Specs ON Specs.ProductId = Products.Id
                        JOIN Categories ON Categories.Id = Products.CategoryId
                        left JOIN ProductImages ON Products.Id = ProductImages.ProductId
                        WHERE Products.Id = @ProductId";

            var queryResult = (await _connection.QueryAsync<ProductQueryResultDTO>(sql, parameter)).ToList();
            if (queryResult.Count == 0)
            {
                return default;
            }
            var product = queryResult[0];
            var result = new ProductDTO
            {
                ProductId = productId,
                CategoryName = product.CategoryName,
                ProductName = product.ProductName,
                Spu = product.Spu,
                Description = product.Description,
                Specs = queryResult.Select(x => new SpecDTO
                {
                    SpecId = x.SpecId,
                    Sku = x.Sku,
                    UnitPrice = x.UnitPrice,
                    SpecName = x.SpecName,
                    SpecOrder = x.SpecOrder,
                    StockQuantity = x.StockQuantity,
                }).ToList(),
                ProductImages = queryResult.Select(PI => new ProductImageDTO
                {
                    ProductImageId = PI.ProductImageId,
                    ProductImageUrl = PI.ProductImageUrl,
                    ProductImageOrder = PI.ProductImageOrder
                }).ToList()
            };

            return result;
        }
    }
}
