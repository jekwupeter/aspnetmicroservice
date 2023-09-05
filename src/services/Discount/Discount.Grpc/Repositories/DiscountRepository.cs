using Dapper;
using Discount.Grpc.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Discount.Grpc.Repositories
{
    public class DiscountRepository : IDiscount
    {
        private readonly NpgsqlDataSourceBuilder _builder;
        private readonly IConfiguration _config;

        public DiscountRepository(IConfiguration config)
        {
            _config = config;
            _builder = new NpgsqlDataSourceBuilder(_config.GetConnectionString("DiscountDb"));
        }
        public async Task<bool> CreateDiscount(Coupon coupon)
        {

            await using var dataSource = _builder.Build();
            await using var conn = await dataSource.OpenConnectionAsync();
            var affected = await conn.ExecuteAsync("INSERT INTO Coupon(ProductName, Description, Amount) VALUES(@ProductName, @Description, @Amount)",
                new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });

            return affected > 0;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            await using var dataSource = _builder.Build();
            await using var conn = await dataSource.OpenConnectionAsync();
            var affected = await conn.ExecuteAsync("DELETE FROM Coupon WHERE ProductName=@ProductName",
                new { ProductName = productName});

            return affected > 0;
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            await using var dataSource = _builder.Build();
            await using var conn = await dataSource.OpenConnectionAsync();
            var coupon = await conn.QueryFirstOrDefaultAsync<Coupon>("SELECT * FROM Coupon WHERE ProductName = @ProductName", new { ProductName = productName });

            if (coupon == null)
                return new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount Desc" };

            return coupon;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            await using var dataSource = _builder.Build();
            await using var conn = await dataSource.OpenConnectionAsync();
            var affected = await conn.ExecuteAsync("UPDATE Coupon SET ProductName=@ProductName, Description=@Description, Amount=@Amount WHERE Id=@Id",
                new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount, Id = coupon.id });

            return affected > 0;
        }
    }
}
