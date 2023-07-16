
namespace Takasbu.Models.DTO
{
public class ProductDTO
{
    public  string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public  string Category { get; set; } = string.Empty;

    public string Picture{ get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}


}
