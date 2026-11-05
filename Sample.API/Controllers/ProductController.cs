using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sample.Application;

namespace Sample.API.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/Products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts(CancellationToken ct)
        {
            var products = await _productService.GetAllProductsAsync(ct);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById([FromRoute] int id, CancellationToken ct)
        {
            var product = await _productService.GetProductByIdAsync(id, ct);
            if (product == null)
            {
                return NotFound("Product not found.");
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] CreateProductDto product, CancellationToken ct)
        {
            if (product == null)
            {
                return BadRequest("Product cannot be null.");
            }

            int newProductId = await _productService.AddProductAsync(product, ct);
            return CreatedAtAction(nameof(GetProductById), new { id = newProductId }, product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id, CancellationToken ct)
        {
            var result = await _productService.DeleteProductAsync(id, ct);
            if (!result)
            {
                return NotFound("Product not found.");
            }
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] UpdateProductDto product, CancellationToken ct)
        {
            if (product == null)
            {
                return BadRequest("Product cannot be null.");
            }

            var result = await _productService.UpdateProductAsync(id, product, ct);
            if (!result)
            {
                return NotFound("Product not found.");
            }
            return Ok();
        }
    }
}
