using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _products;
       
        public ProductRepository(IConfiguration config)
        {
            var db = new MongoClient(config["DbSettings:mongo-url"]).GetDatabase(config["DbSettings:mongo-db"]);
            _products = db.GetCollection<Product>(config["DbSettings:mongo-collection"]);
            CatalogContextSeed.SeedData(_products);
        }
        public async Task CreateProduct(Product product)
        {
            await _products.InsertOneAsync(product);
        }

        public async Task<bool> DeleteProduct(string id)
        {
            var deleteResult =  await _products.DeleteOneAsync(id);


            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<Product> GetProduct(string id)
        {
            return await _products.Find(p => p.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProductbyCategory(string categoryName)
        {
            return await _products.Find(p => p.Category.Equals(categoryName)).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductbyName(string name)
        {
            return await _products.Find(p => p.Name.Equals(name)).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _products.Find(x => true).ToListAsync();
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            var updateResult = await _products.ReplaceOneAsync(x => x.Id == product.Id, product);

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }
    }
}
