﻿using Dapper;
using Npgsql;

namespace Discount.Grpc.Extensions
{
    public static class MiddlewareExtensions
    {
        public static async Task<WebApplication> ApplyMigrationsAsync(this WebApplication app, int? retry = 0)
        {
            int retryForAvailability = retry.Value;

            using var scope = app.Services.CreateScope();
            var config = app.Configuration;
            var log = app.Logger;
            var builder = new NpgsqlDataSourceBuilder(config.GetConnectionString("DiscountDb"));


            try 
            {
                log.LogInformation("Migrating postgresql database");
                await using var dataSource = builder.Build();
                await using var conn = await dataSource.OpenConnectionAsync();

                int affectedCount = 0;
                affectedCount = await conn.ExecuteAsync("DROP TABLE IF EXISTS Coupon");

                affectedCount = await conn.ExecuteAsync(@"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY, 
                                                                ProductName VARCHAR(24) NOT NULL,
                                                                Description TEXT,
                                                                Amount INT)");

                affectedCount = await conn.ExecuteAsync("INSERT INTO Coupon (ProductName, Description, Amount) VALUES ('IPhone X', 'IPhone Discount', 150)");

                affectedCount = await conn.ExecuteAsync("INSERT INTO Coupon (ProductName, Description, Amount) VALUES ('Samsung 10', 'Samsung Discount', 100)");



                log.LogInformation("Completed  migration to postgresql database.");
            }
            catch(NpgsqlException ex) 
            {
                log.LogError("Error occured while applying pgsql migration. Ex: {ex}", ex.Message);
                if (retryForAvailability < 50)
                {
                    retryForAvailability++;
                    Thread.Sleep(2000);
                    await ApplyMigrationsAsync(app, retryForAvailability);
                }
            }

            return app;
        }
    }
}
