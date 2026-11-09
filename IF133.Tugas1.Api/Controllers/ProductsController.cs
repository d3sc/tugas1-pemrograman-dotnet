using System.ComponentModel.DataAnnotations;
using IF133.Tugas1.Api.Data;
using IF133.Tugas1.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApi.Dtos;

namespace IF133.Tugas1.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private static ProductReadDto ToReadDto(Product product)
    {
        return new ProductReadDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price
        };
    }

    private readonly AppDbContext _context;
    private readonly ILogger<ProductsController> _logger;
    public ProductsController(AppDbContext context, ILogger<ProductsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET /api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        var products = await _context.Products
            // menambahkan DTO untuk membaca data produk
            .Select(p => new ProductReadDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price
            })
            .AsNoTracking()
            .ToListAsync();

        return Ok(products);
    }

    // GET /api/products/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            _logger.LogWarning("Produk {Id} tidak ditemukan", id);
            return NotFound(new ProblemDetails
            {
                Title = "Produk tidak ditemukan",
                Detail = $"Produk dengan id {id} tidak ditemukan.",
                Status = StatusCodes.Status404NotFound
            });
        }

        _logger.LogInformation("Produk {Id} ditemukan", id);

        return Ok(ToReadDto(product));
    }

    // POST /api/products
    [HttpPost]
    public async Task<ActionResult<ProductReadDto>> CreateProduct(
        CreateProductDto dto)
    {
        if (dto.Price is <= 0 || dto.Stock is <= 0)
        {
            return BadRequest(new
            {
                message = "Harga dan stok wajib diisi."
            });
        }

        var product = new Product
        {
            Name = dto.Name.Trim(),
            Description = dto.Description,
            Price = dto.Price,
            Stock = (int)dto.Stock
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created product {Id} - {Name}", product.Id, product.Name);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            ToReadDto(product)
        );
    }

    // PUT /api/products/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateProductDto dto)
    {
        if (dto.Price is <= 0 || dto.Stock is <= 0)
        {
            return BadRequest(new
            {
                message = "Harga dan stok wajib diisi."
            });
        }

        var product = await _context.Products.FindAsync(id);

        if (product is null)
        {
            return NotFound(new
            {
                message = "Produk tidak ditemukan."
            });
        }

        product.Name = dto.Name.Trim();
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Stock = (int)dto.Stock;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Produk {Id} - {Name} diubah", id, product.Name);

        return Ok(ToReadDto(product));
    }

    // DELETE /api/products/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null)
        {
            _logger.LogWarning("Produk {Id} tidak ditemukan", id);
            return NotFound(new ProblemDetails
            {
                Title = "Produk tidak ditemukan",
                Detail = $"Produk dengan id {id} tidak ditemukan.",
                Status = StatusCodes.Status404NotFound
            });
        }

        _logger.LogInformation("Produk {Id} - {Name} dihapus", id, product.Name);

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

// DTO untuk input POST dan PUT.
// Id dibuat oleh database, sehingga tidak diminta dari pengguna.
public class ProductRequest
{
    [Required(ErrorMessage = "Nama produk wajib diisi.")]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Harga wajib diisi.")]
    [Range(typeof(decimal), "0", "79228162514264337593543950335",
        ErrorMessage = "Harga tidak boleh negatif.")]
    public decimal? Price { get; set; }

    [Required(ErrorMessage = "Stok wajib diisi.")]
    [Range(0, int.MaxValue, ErrorMessage = "Stok tidak boleh negatif.")]
    public int? Stock { get; set; }
}