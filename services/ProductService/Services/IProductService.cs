using ProductService.DTOs;

namespace ProductService.Services
{
    public interface IProductService
    {
        Task<ProductResponseDto> CreateProductAsync(ProductCreateDto dto);

        Task<IEnumerable<ProductResponseDto>> GetProductsAsync();

        Task<ProductResponseDto?> GetByIdAsync(int id);

        Task<bool> UpdateAsync(int id, UpdateProductDto dto);
        Task<bool> DeleteAsync(int id);
    }
}