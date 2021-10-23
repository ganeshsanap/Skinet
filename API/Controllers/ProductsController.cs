using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts()
        {
            var products = await _productRepository.GetProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            return Ok(product);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            var productBrands = await _productRepository.GetProductBrandsAsync();
            return Ok(productBrands);
        }

        [HttpGet("brands/{id:int}")]
        public async Task<ActionResult<ProductBrand>> GetProductBrand(int id)
        {
            var productBrand = await _productRepository.GetProductBrandByIdAsync(id);
            return Ok(productBrand);
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductTypes()
        {
            var productTypes = await _productRepository.GetProductTypesAsync();
            return Ok(productTypes);
        }

        [HttpGet("types/{id:int}")]
        public async Task<ActionResult<ProductType>> GetProductType(int id)
        {
            var productType = await _productRepository.GetProductTypeByIdAsync(id);
            return Ok(productType);
        }
    }
}
