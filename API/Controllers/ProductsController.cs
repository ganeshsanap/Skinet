using API.DTOs;
using API.Errors;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<ProductBrand> _productBrnadRepository;
        private readonly IGenericRepository<ProductType> _productTypeRepository;
        private readonly IMapper _mapper;

        public ProductsController(IGenericRepository<Product> productRepository,
            IGenericRepository<ProductBrand> productBrnadRepository,
            IGenericRepository<ProductType> productTypeRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _productBrnadRepository = productBrnadRepository;
            _productTypeRepository = productTypeRepository;
            _mapper = mapper;
        }

        [Cached(600)]
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDTO>>> GetProducts(
            [FromQuery] ProductSpecParams productSpecParams)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(productSpecParams);
            var products = await _productRepository.ListAsync(spec);

            var countSpec = new ProductsWithFiltersForCountSpecification(productSpecParams);
            var totalItems = await _productRepository.CountAsync(countSpec);
           
            var productsToReturnDTO = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDTO>>(products);

            return Ok(new Pagination<ProductToReturnDTO>(productSpecParams.PageIndex, productSpecParams.PageSize, totalItems, productsToReturnDTO));
        }

        [Cached(600)]
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDTO>> GetProduct(int id)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);

            var product = await _productRepository.GetEntityWithSpec(spec);

            if (product == null)
                return NotFound(new ApiResponse(404));

            var productToReturnDTO = _mapper.Map<Product, ProductToReturnDTO>(product);

            return Ok(productToReturnDTO);
        }

        [Cached(600)]
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            var productBrands = await _productBrnadRepository.ListAllAsync();
            return Ok(productBrands);
        }

        [Cached(600)]
        [HttpGet("brands/{id:int}")]
        public async Task<ActionResult<ProductBrand>> GetProductBrand(int id)
        {
            var productBrand = await _productBrnadRepository.GetByIdAsync(id);
            return Ok(productBrand);
        }

        [Cached(600)]
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductTypes()
        {
            var productTypes = await _productTypeRepository.ListAllAsync();
            return Ok(productTypes);
        }

        [Cached(600)]
        [HttpGet("types/{id:int}")]
        public async Task<ActionResult<ProductType>> GetProductType(int id)
        {
            var productType = await _productTypeRepository.GetByIdAsync(id);
            return Ok(productType);
        }
    }
}
