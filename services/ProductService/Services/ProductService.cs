using ProductService.DTOs;
using ProductService.Models;
using ProductService.Repositories;

namespace ProductService.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProductResponseDto> CreateProductAsync(ProductCreateDto dto)
        {
            var product = new Product
            {
                Sku = dto.Sku,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price
            };

            var created = await _repository.AddAsync(product);

            return new ProductResponseDto
            {
                Id = created.Id,
                Sku = created.Sku,
                Name = created.Name,
                Description = created.Description,
                Price = created.Price,
                IsActive = created.IsActive,
                CreatedAt = created.CreatedAt
            };
        }

        public async Task<IEnumerable<ProductResponseDto>> GetProductsAsync()
        {
            var products = await _repository.GetAllAsync();

            return products.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Sku = p.Sku,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            });
        }
        public async Task<ProductResponseDto?> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
                return null;

            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Sku = product.Sku,
                Description = product.Description,
                IsActive = product.IsActive,
                Price = product.Price,
            };
        }
        public async Task<bool> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
                return false;

            product.Name = dto.Name;
            product.Sku = dto.Sku;
            product.Description = dto.Description;
            product.IsActive = dto.IsActive;

            await _repository.UpdateAsync(product);

            return true;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null || product.IsActive == false)
                return false;

           product.IsActive = false;

            await _repository.UpdateAsync(product);

            return true;
        }
    }
}