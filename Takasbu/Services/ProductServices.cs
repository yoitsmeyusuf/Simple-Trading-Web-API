// <snippet_File>
using Takasbu.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Takasbu.Services;

public class ProductService
{
    private readonly IMongoCollection<Product> _ProductsCollection;

    // <snippet_ctor>
    public ProductService(
        IOptions<ProductStoreDatabaseSettings> ProductStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            ProductStoreDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            ProductStoreDatabaseSettings.Value.DatabaseName);

        _ProductsCollection = mongoDatabase.GetCollection<Product>(
          ProductStoreDatabaseSettings.Value.ProductsCollectionName);
    }
    // </snippet_ctor>

    public async Task<List<Product>> GetAsync() =>
        await _ProductsCollection.Find(_ => true).ToListAsync();

    public async Task<Product?> GetAsync(string id) =>
        await _ProductsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
  public async Task<List<Product>> GetListAsync(List<string> ids)
{
    var filter = Builders<Product>.Filter.In(x => x.Id, ids);
    var result = await _ProductsCollection.Find(filter).ToListAsync();
    return result;
}
          public async Task<Product?> GetAsyncu(string id) =>
        await _ProductsCollection.Find(x => x.Name == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Product newProduct) =>
        await _ProductsCollection.InsertOneAsync(newProduct);

    public async Task UpdateAsync(string id, Product updatedProduct) =>
        await _ProductsCollection.ReplaceOneAsync(x => x.Id == id, updatedProduct);
      public async Task UpdateAsyncu(string id, Product updatedProduct) =>
        await _ProductsCollection.ReplaceOneAsync(x => x.Name == id, updatedProduct);
    public async Task RemoveAsync(string id) =>
        await _ProductsCollection.DeleteOneAsync(x => x.Id == id);
}
// </snippet_File>
