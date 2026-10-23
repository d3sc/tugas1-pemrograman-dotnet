using System.ComponentModel.DataAnnotations;
using IF133.Tugas1.Api.Data;
using IF133.Tugas1.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IF133.Tugas1.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    // GET /api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        var products = await _context.Products
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
            return NotFound(new { message = "Produk tidak ditemukan." });
        }

        return Ok(product);
    }

    // POST /api/products
    [HttpPost]
    public async Task<ActionResult<Product>> Create(
        [FromBody] ProductRequest request)
    {
        if (request.Price is not decimal price ||
            request.Stock is not int stock)
        {
            return BadRequest(new
            {
                message = "Harga dan stok wajib diisi."
            });
        }

        var product = new Product
        {
            Name = request.Name.Trim(),
            Description = request.Description,
            Price = price,
            Stock = stock
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product
        );
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] ProductRequest request)
    {
        if (request.Price is not decimal price ||
            request.Stock is not int stock)
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

        product.Name = request.Name.Trim();
        product.Description = request.Description;
        product.Price = price;
        product.Stock = stock;

        await _context.SaveChangesAsync();

        return Ok(product);
    }

    // DELETE /api/products/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null)
        {
            return NotFound(new { message = "Produk tidak ditemukan." });
        }

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