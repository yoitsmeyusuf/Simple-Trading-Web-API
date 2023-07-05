// <snippet_File>
namespace Takasbu.Services
{
    public class ProductStoreDatabaseSettings
    {
        
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string ProductsCollectionName { get; set; } = null!;
    }
}