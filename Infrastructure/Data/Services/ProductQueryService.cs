using ApplicationCore.DTOs.ProductDTOs;
using ApplicationCore.Entities;
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
            var getProductsQuerySql = @"WITH MinUnitPrice_CTE(ProductId, MinUnitPrice)
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
                                            (
		                                        SELECT TOP 1
			                                        ProductImages.URL
		                                        FROM ProductImages
		                                        WHERE ProductImages.ProductId = Products.Id
		                                        ORDER BY ProductImages.[Order]
	                                        ) AS ProductImageUrl
                                        FROM Products 
                                        JOIN MinUnitPrice_CTE ON MinUnitPrice_CTE.ProductId = Products.Id
                                        JOIN Categories ON Categories.Id = Products.CategoryId 
                                        WHERE Categories.Id = @CategoryId
                                        AND Products.IsDelete = 0
                                        AND Products.Enable = 1
                                        ORDER BY Products.[Order], Products.CreateAt";

            return (await _connection.QueryAsync<GetProductListDTO>(getProductsQuerySql, parameter)).ToList();
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
	                        Specs.[Order] AS SpecOrder
                        FROM Products
                        JOIN Specs ON Specs.ProductId = Products.Id
                        JOIN Categories ON Categories.Id = Products.CategoryId
                        WHERE Products.Id = @ProductId
                        AND Products.IsDelete = 0
                        ORDER BY Products.[Order], Products.CreateAt, SpecOrder, Specs.CreateAt";

            var queryResult = (await _connection.QueryAsync<ProductQueryResultDTO>(sql, parameter)).ToList();

            if (queryResult.Count == 0) return null;

            var getProductImagesSQL = @"SELECT 
	                                        Id AS ProductImageId,
	                                        URL AS ProductImageUrl,
	                                        [Order] AS ProductImageOrder
                                        FROM ProductImages
                                        WHERE ProductId = @ProductId
                                        ORDER BY ProductImageOrder";
            var productImagesQueryResult = (await _connection.QueryAsync<ProductImageDTO>(getProductImagesSQL, parameter)).ToList();

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
                ProductImages = productImagesQueryResult
            };

            return result;
        }
    }
}
