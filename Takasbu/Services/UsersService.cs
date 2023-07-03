// <snippet_File>
using Takasbu.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Takasbu.Services;

public class UsersService
{
    private readonly IMongoCollection<User> _UsersCollection;

    // <snippet_ctor>
    public UsersService(
        IOptions<UserStoreDatabaseSettings> UserStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            UserStoreDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            "BookStore");

        _UsersCollection = mongoDatabase.GetCollection<User>(
            "Users");
    }
    // </snippet_ctor>

    public async Task<List<User>> GetAsync() =>
        await _UsersCollection.Find(_ => true).ToListAsync();

    public async Task<User?> GetAsync(string id) =>
        await _UsersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
          public async Task<User?> GetAsyncu(string id) =>
        await _UsersCollection.Find(x => x.Username == id).FirstOrDefaultAsync();

    public async Task CreateAsync(User newUser) =>
        await _UsersCollection.InsertOneAsync(newUser);

    public async Task UpdateAsync(string id, User updatedUser) =>
        await _UsersCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

    public async Task RemoveAsync(string id) =>
        await _UsersCollection.DeleteOneAsync(x => x.Id == id);
}
// </snippet_File>
