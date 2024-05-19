        using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Specifications.Products_Specs;

namespace Talabat.APIs.Controllers
{

    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<ProductBrand> _brandsRepo;
        private readonly IGenericRepository<ProductType> _categoriesRepo;

        public ProductsController(IGenericRepository<Product> productRepo,IMapper mapper,IGenericRepository<ProductBrand> brandsRepo,
            IGenericRepository<ProductType> categoriesRepo)
        {
            _productRepo = productRepo;
            _mapper = mapper;
            _brandsRepo = brandsRepo;
            _categoriesRepo = categoriesRepo;
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecsParams specsParams)
        {
            //var products = await _productRepo.GetAllAsync();
             var spec = new ProductWithBrandAndCategorySpecifications(specsParams);
            var products = await _productRepo.GetWithSpecAllAsync(spec);
            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);
            var countSpec = new ProductWithFilterationForCountSpecifications(specsParams);
            int count = await _productRepo.GetCountAsync(countSpec);

            return Ok(new Pagination<ProductToReturnDto>(specsParams.PageSize,specsParams.PageIndex, count, data));
        }

        [ProducesResponseType(typeof(ProductToReturnDto),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            //var product = await _productRepo.GetAsync(id);
            var specs = new ProductWithBrandAndCategorySpecifications(id);
            var product = await _productRepo.GetWithSpecAsync(specs);
            if (product is null)
            {
                return NotFound(new { Message = "Not Found", StatusCode = 404 });
            }

            var result = _mapper.Map<Product, ProductToReturnDto>(product);
            return Ok(result);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IEnumerable<ProductBrand>>> GetBrands()
        {
            var brands = await _brandsRepo.GetAllAsync();
            return Ok(brands);
        }

        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<ProductType>>> GetCategories()
        {
            var categories = await _categoriesRepo.GetAllAsync();
            return Ok(categories);
        }



    }
}
