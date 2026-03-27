using Microsoft.AspNetCore.Http.HttpResults;
using ProductService.DTOs;
using ProductService.Models;
using ProductService.Repositories;

namespace ProductService.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository repository, ILogger<ProductService> logger )
        {
            _repository = repository;
            _logger = logger;
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

            _logger.LogInformation("Product {Id} created successfully with SKU {Sku}, name {Name} and price {Price}",
                created.Id, created.Sku, created.Name, created.Price);

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

            if (product is null)
            {
                _logger.LogWarning("Product {ProductId} not found", id);
                return null;
            }

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

            if (product is null)
            {
                _logger.LogWarning("Product {ProductId} not found", id);
                return false;
            }

            product.Name = dto.Name;
            product.Sku = dto.Sku;
            product.Price = dto.Price;
            product.Description = dto.Description!;
            product.IsActive = dto.IsActive;

            await _repository.UpdateAsync(product);

            _logger.LogInformation("Product {Id} Updated successfully with SKU {Sku}, name {Name} and price {Price}",
                id, dto.Sku, dto.Name, dto.Price);

            return true;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null || product.IsActive == false)
            {
                _logger.LogWarning("Product {ProductId} not found or is disable", id);
                return false;
            }

            product.IsActive = false;

            await _repository.UpdateAsync(product);
            _logger.LogInformation("Product {Id} Disabled successfully",
                id);

            return true;
        }
    }
}